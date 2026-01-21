import MakeProblem as mp
import Graphing as gp
import cv2
import matplotlib.pyplot as plt
import numpy as np
from dataclasses import dataclass, field
from typing import List
from google.colab.patches import cv2_imshow
from QAOAMethod import Method
import matplotlib.pyplot as plt
from k_means_constrained import KMeansConstrained
from Sampling import *
import pickle
from itertools import combinations
import os
from sklearn.neighbors import KDTree
import random


class applicationQAOA:
  
  
  class imageSegmetation:

    """
    Just a class of functions that use QAOA to solve image segmentation.

    Funtions
    --------
    ->contrast_stretch : Preprocessing image to clearer regions.
    ->updateBranchStatus : A checker that determines patches is finished or not.
    ->applyMaskArray : Updating algorithm output mask, according to this recursive solve.
    ->keepDoingPatches : When a patch not finished, this functions growth patch's
                child nodes to keep recursive solving it.
    ->create3x3Patches : Determine how to divide numRowPatch * numRowPatch,
                in each small region or whole image.
    ->execute : Like a main function for this application.

    Parameters
    ----------
    root : string
        An input data path please provided .jpg not folder.
        **default = "/content/drive/MyDrive/Colab Notebooks/QAOAAllinOne/855_sat_12.jpg"**
    mask : string 
        An experiment input data ground turth path,
        for preformance analysis please provided .jpg not folder.
        **default = "/content/drive/MyDrive/Colab Notebooks/QAOAAllinOne/855_mask_12.jpg"**
    sigma : double/float/int
        Standard deviation for gaussian similarity metric.
        **default = 1**
    numRowPatch : int
        Each recursive solving, divided to how many patches.
        numRowPatch * numRowPatch. Recommended: 3.
        **default = 3**
    """

    ################################################################################################################################

    # Binary tree structure for patches of pixel)
    @dataclass
    class treeRoot:
      # storing a starting branch of starting patches pixel
      followingBranches: List["applicationQAOA.imageSegmetation.branch"] = field(default_factory=list)

    @dataclass
    class branch:
      status: str = "" # finish or ready to be cut
      maskArray: np.ndarray = field(default_factory=lambda: np.array([]))  # this patch mask outcome
      pixelTopLeftIndex: List[int] = field(default_factory=list) # This patches to original coordinate(Top Left)
      pixelBottomRightIndex: List[int] = field(default_factory=list) # This patches to original coordinate(Bottom right)
      pixelPatchValues: int = 0 # patch mean of light level
      cutResult: str = "" # for when height < 3 or width < 3 apply the original cut directly
      followingBranches: List["applicationQAOA.imageSegmetation.branch"] = field(default_factory=list)

    ################################################################################################################################

    ################################################################################################################################

    # For initialize the problem we are dealing

    def __init__(
          self,
          root="/content/drive/MyDrive/Colab Notebooks/QAOAAllinOne/855_sat_12.jpg",
          mask="/content/drive/MyDrive/Colab Notebooks/QAOAAllinOne/855_mask_12.jpg",
          sigma=1,
          numRowPatch=3
        ):
        # error handler if image doesn't exist on root
        try:
          # Original image
          self.image = cv2.imread(root, 0)
          # Ground truth
          self.mask = cv2.imread(mask, 0)
        except:
          print("Error loading images.. use original")
          self.image = cv2.imread("/content/drive/MyDrive/Colab Notebooks/QAOAAllinOne/855_sat_12.jpg", 0)
          self.mask = cv2.imread("/content/drive/MyDrive/Colab Notebooks/QAOAAllinOne/855_sat_12.jpg", 0)

        self.width = self.image.shape[0]
        self.height = self.image.shape[1]
        self.algorithmMask = np.zeros((self.image.shape[0], self.image.shape[1]))
        self.meansLevelOfOnesGroup = 0
        self.meansLevelOfZerosGroup = 0
        self.sigma = sigma
        print(self.image.shape)
        print(self.algorithmMask.shape)
        self.numRowPatch = numRowPatch
    
    """
    Contrast stretching, making image regions clearer.
    Also, set sigma according to after stretching image.

    Parameters
    ----------
    picture : string (required)
        An input data path please provided .jpg not folder.
    
    Returns
    -------
    stretchedImage : numpy array
        After stretched image.
    """
    def contrast_stretch(self, picture):
      cv2_imshow(picture)
      plt.hist(picture.ravel(), 256, [0, 256])

      # Find the minimum and maximum pixel values in the image
      min_val = np.min(picture)
      max_val = np.max(picture)

      # Stretch the pixel values to the full range [0, 255]
      stretched_image = (self.image - min_val) * (255 / (max_val - min_val))
      cv2_imshow(stretched_image.astype(np.uint8))
      plt.hist(stretched_image.ravel(), 256, [0, 256])
      plt.show()

      # clear plt
      plt.clf()

      # ----- Compute σ_histogram -----

      Imax = 255
      Imin = 0
      Imid = Imax // 2

      histogramStretchedImage, _ = np.histogram(stretchedImage, bins=256, range=(0, 256))

      # I_L = [0, Imid), I_R = (Imid, Imax]
      I_L = histogramStretchedImage[0: Imid]
      I_R = histogramStretchedImage[Imid + 1: Imax + 1]

      # Max voted intensity each region
      maxIntensityL = np.argmax(I_L)
      maxIntensityR = np.argmax(I_R) + Imid + 1

      # Calculate sigma
      sigma_histogram = maxIntensityR - maxIntensityL 
      self.sigma = sigma_histogram
      print(sigma_histogram)
      print(self.sigma)

      return stretchedImage.astype(np.uint8)

    ################################################################################################################################

    # determine the outcome of the mask by QAOA response
    """
    Do not call this method.
    """

    def updateBranchStatus(self, bestBinary, parentBranch, Iter="Other"):
        
        # Group branches into "remain needs to be cut" or "finish"
        onesGroup = [i for i, value in enumerate(bestBinary) if value == '1']
        zerosGroup = [i for i, value in enumerate(bestBinary) if value == '0']

        if Iter == "Initial":
          onesGroupLightLevel = []
          zerosGroupLightLevel = []
          # most group should be '1' if not swap the array
          # flip the element of cutBinary '0' to '1' & '1' to '0'
          if len(onesGroup) < len(zerosGroup):
            onesGroup, zerosGroup = zerosGroup, onesGroup
            bestBinary = ''.join('1' if x == '0' else '0' for x in bestBinary)

            """
            create individual one's group lenght of topleft[0]-bottomright[0]
            width and topleft[1]-bottomright[1] height array mask with only '1' element.
            
            create individual zero's group lenght of topleft[0]-bottomright[0]
            width and topleft[1]-bottomright[1] height array mask with only  '0' element.
            """

            for _, i in enumerate(onesGroup):
              parentBranch.followingBranches[i].status = "finish"
              parentBranch.followingBranches[i].maskArray = np.ones((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
              ))
              onesGroupLightLevel.append(parentBranch.followingBranches[i].pixelPatchValues)

            self.meansLevelOfOnesGroup = np.mean(onesGroupLightLevel)

            for _, i in enumerate(zerosGroup):
              parentBranch.followingBranches[i].status = "remain needs to be cut"
              parentBranch.followingBranches[i].maskArray = np.zeros((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
              ))
              zerosGroupLightLevel.append(parentBranch.followingBranches[i].pixelPatchValues)

            self.meansLevelOfZerosGroup = np.mean(zerosGroupLightLevel)

            self.applyMaskArray(parentBranch)

          else:

            """
            create individual one's group lenght of topleft[0]-bottomright[0]
            width and topleft[1]-bottomright[1] height array mask with only '1' element.

            create individual zero's group lenght of topleft[0]-bottomright[0]
            width and topleft[1]-bottomright[1] height array mask with only  '0' element.
            """
            for i in onesGroup:
              parentBranch.followingBranches[i].status = "finish"
              parentBranch.followingBranches[i].maskArray = np.ones((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
              ))
              onesGroupLightLevel.append(parentBranch.followingBranches[i].pixelPatchValues)

            self.meansLevelOfOnesGroup = np.mean(onesGroupLightLevel)

            for i in zerosGroup:
              parentBranch.followingBranches[i].status = "remain needs to be cut"
              parentBranch.followingBranches[i].maskArray = np.zeros((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
              ))
              zerosGroupLightLevel.append(parentBranch.followingBranches[i].pixelPatchValues)

            self.meansLevelOfZerosGroup = np.mean(zerosGroupLightLevel)

            self.applyMaskArray(parentBranch)

          # print(self.meansLevelOfOnesGroup)

        else: # Iter == "Other"

          # if group is empty take the none empty one to mean and compare the distance
          if len(onesGroup) == 0:
            zerosGroupLightLevel = np.mean([
              parentBranch.followingBranches[values].pixelPatchValues for _, values in enumerate(zerosGroup)
            ])
            if abs(self.meansLevelOfOnesGroup - zerosGroupLightLevel) > abs(self.meansLevelOfZerosGroup - zerosGroupLightLevel):
              for i in zerosGroup:
                parentBranch.followingBranches[i].status = "finish"
                parentBranch.followingBranches[i].maskArray = np.zeros((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
                ))
            else: # abs(self.meansLevelOfOnesGroup - existGroupMean) < abs(self.meansLevelOfZerosGroup - existGroupMean)
              for i in zerosGroup:
                parentBranch.followingBranches[i].status = "finish"
                parentBranch.followingBranches[i].maskArray = np.ones((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
                ))

            self.applyMaskArray(parentBranch)

          elif len(zerosGroup) == 0:
            onesGroupLightLevel = np.mean([
              parentBranch.followingBranches[values].pixelPatchValues for _, values in enumerate(onesGroup)
            ])
            if abs(self.meansLevelOfOnesGroup - onesGroupLightLevel) > abs(self.meansLevelOfZerosGroup - onesGroupLightLevel):
              for i in onesGroup:
                parentBranch.followingBranches[i].status = "finish"
                parentBranch.followingBranches[i].maskArray = np.zeros((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
                ))
            else: # abs(self.meansLevelOfOnesGroup - existGroupMean) < abs(self.meansLevelOfZerosGroup - existGroupMean)
              for i in onesGroup:
                parentBranch.followingBranches[i].status = "finish"
                parentBranch.followingBranches[i].maskArray = np.ones((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
                ))

            self.applyMaskArray(parentBranch)
          else:

            onesGroupLightLevel = np.mean([
              parentBranch.followingBranches[values].pixelPatchValues for _, values in enumerate(onesGroup)
            ])
            zerosGroupLightLevel = np.mean([
              parentBranch.followingBranches[values].pixelPatchValues for _, values in enumerate(zerosGroup)
            ])
            
            if abs(self.meansLevelOfOnesGroup - onesGroupLightLevel) > abs(self.meansLevelOfOnesGroup - zerosGroupLightLevel):

              #swap
              zerosGroup, onesGroup = onesGroup, zerosGroup
              bestBinary = ''.join('1' if x == '0' else '0' for x in bestBinary)

              for i in zerosGroup:
                parentBranch.followingBranches[i].status = (
                  "remain needs to be cut" if len(onesGroup) > len(zerosGroup) else "finish"
                )
                parentBranch.followingBranches[i].maskArray = np.zeros((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
                ))

              for i in onesGroup:
                parentBranch.followingBranches[i].status = (
                  "finish" if len(onesGroup) > len(zerosGroup) else "remain needs to be cut"
                )
                parentBranch.followingBranches[i].maskArray = np.ones((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
                ))

              self.applyMaskArray(parentBranch)

            else: # abs(self.meansLevelOfOnesGroup - onesGroupLightLevel) < abs(self.meansLevelOfOnesGroup - zerosGroupLightLevel):

              for _, i in enumerate(onesGroup):
                parentBranch.followingBranches[i].status = (
                  "finish" if len(onesGroup) > len(zerosGroup) else "remain needs to be cut"
                )
                parentBranch.followingBranches[i].maskArray = np.ones((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
                ))

              for _, i in enumerate(zerosGroup):
                parentBranch.followingBranches[i].status = (
                  "remain needs to be cut" if len(onesGroup) > len(zerosGroup) else "finish"
                )
                parentBranch.followingBranches[i].maskArray = np.zeros((
                  parentBranch.followingBranches[i].pixelBottomRightIndex[0] - parentBranch.followingBranches[i].pixelTopLeftIndex[0],
                  parentBranch.followingBranches[i].pixelBottomRightIndex[1] - parentBranch.followingBranches[i].pixelTopLeftIndex[1]
                ))

              self.applyMaskArray(parentBranch)

        # slow
        # cv2_imshow(self.algorithmMask)

        # print("i am alive")

    ################################################################################################################################

    # used by updateStatus to modify the solution output
    """
    Do not call this method.
    """
    def applyMaskArray(self, parentsBranch, insufficientSize=False):
      # If finish indicate cant be cut again
      if insufficientSize == False:
        for branch in parentsBranch.followingBranches:
          # print(branch.pixelTopLeftIndex[0], branch.pixelBottomRightIndex[0])
          # print(branch.pixelTopLeftIndex[1], branch.pixelBottomRightIndex[1])
          self.algorithmMask[
            branch.pixelTopLeftIndex[0]:branch.pixelBottomRightIndex[0],
            branch.pixelTopLeftIndex[1]:branch.pixelBottomRightIndex[1]
          ] = branch.maskArray * 255
      else:
        self.algorithmMask[
          parentsBranch.pixelTopLeftIndex[0]:parentsBranch.pixelBottomRightIndex[0],
          parentsBranch.pixelTopLeftIndex[1]:parentsBranch.pixelBottomRightIndex[1]
        ] = parentsBranch.maskArray * 255

    ################################################################################################################################

    # a recursive procedure for none finish branch to keep growing up
    """
    Do not call this method.
    """
    def keepDoingPatches(
        self, parentsBranch, method, wMaxValueWholeImage,
        wMinimumValueWholeImage, enhancedAdj, flipAdjWeight
        ):
      for branch in parentsBranch.followingBranches:
        # print(branch.status)
        # Cal if branch segment is smaller than 3x3
        width = branch.pixelBottomRightIndex[0] - branch.pixelTopLeftIndex[0]
        height = branch.pixelBottomRightIndex[1] - branch.pixelTopLeftIndex[1]

        # print(width, height)
        if width < self.numRowPatch or height < self.numRowPatch:
          branch.status = "finish"
          # self.applyMaskArray(branch, insufficientSize=True)
          continue

        if branch.status == "remain needs to be cut":
          # print("debug")
          # print("start", branch.pixelTopLeftIndex)
          # print("end", branch.pixelBottomRightIndex)

          # branch's portion of the image
          imageSection = self.image[
              # logically cv2 getting row first so RowStarts to RowEnd
              branch.pixelTopLeftIndex[0]:branch.pixelBottomRightIndex[0],
              branch.pixelTopLeftIndex[1]:branch.pixelBottomRightIndex[1]
          ]

          # print("shape", imageSection.shape)

          # create new patches
          self.create3x3Patches(image=imageSection, parentBranch=branch)

          # print(f"Branch at {branch.pixelTopLeftIndex} has {len(branch.followingBranches)} following branches.")

          adjMatrix, _, _ = mp.imageToAdj(
              branch.followingBranches, numIter=2, sigma=self.sigma,
              wMaxValue=wMaxValueWholeImage, wMinimumValue=wMinimumValueWholeImage
              , enhancedAdj=enhancedAdj, flipAdjWeight=flipAdjWeight
          )

          method.problemConfig(N=len(adjMatrix), adjMatrix=adjMatrix)
          _, _, best, numIter, bestBinary = method.executeLayerWise()

          # gp.showCutedGraph(group=bestBinary, adjMatrix=adjMatrix, layout="grid_layout")

          self.updateBranchStatus(bestBinary, branch)
          self.keepDoingPatches(
              branch, method, wMaxValueWholeImage,
              wMinimumValueWholeImage, enhancedAdj=enhancedAdj,
              flipAdjWeight=flipAdjWeight
              )


    ################################################################################################################################
    """
    Do not call this method.
    """
    def create3x3Patches(self, image, parentBranch, Iter="Other"):
      height, width = image.shape

      # Ensure image dimensions allow at least a 3x3 grid
      if width < self.numRowPatch or height < self.numRowPatch:
        print(f"Branch too small to subdivide: width={width}, height={height}")
        return

      patchWidth = width // self.numRowPatch
      patchHeight = height // self.numRowPatch
      # print(patchWidth, patchHeight)
      lastRowPatchHeight = height - ((self.numRowPatch - 1) * patchHeight)  # ensure takes all row
      lastColPatchWidth = width - ((self.numRowPatch - 1) * patchWidth)  # ensure takes all col

      # print("debug")
      # print("width", width)
      # print("height", height)
      # print("patchWidth", patchWidth)
      # print("patchHeight", patchHeight)
      # print("lastRowPatchHeight", lastRowPatchHeight)
      # print("lastColPatchWidth", lastColPatchWidth)

      for row in range(self.numRowPatch):
        for col in range(self.numRowPatch):
          patchesAllPixelLightLevel = []

          # define starting topLeft and bottomRight coor respect to Original image
          rowStart = row * patchHeight + (
              parentBranch.pixelTopLeftIndex[0]
              if Iter == "Other" else 0
          )
          colStart = col * patchWidth + (
              parentBranch.pixelTopLeftIndex[1]
              if Iter == "Other" else 0
          )
          rowEnd = rowStart + (
              patchHeight if row < self.numRowPatch - 1 else lastRowPatchHeight
          )
          colEnd = colStart + (
              patchWidth if col < self.numRowPatch - 1 else lastColPatchWidth
          )

          # get the pixel
          for rowPixel in range(row * patchHeight,
                      row * patchHeight + patchHeight
                      if row < self.numRowPatch - 1
                      else image.shape[0]):
            for colPixel in range(col * patchWidth,
                      col * patchWidth + patchWidth
                      if col < self.numRowPatch - 1
                      else image.shape[1]):
                patchesAllPixelLightLevel.append(image[rowPixel, colPixel])


          # print("debug")
          # print("start", rowStart, colStart)
          # print("end", rowEnd, colEnd)

          # Create new branch
          childBranch = self.branch()
          # mean or median
          childBranch.pixelPatchValues = np.mean(patchesAllPixelLightLevel)
          # childBranch.pixelPatchValues = np.median(patchesAllPixelLightLevel)

          # assign branch patches properties
          childBranch.pixelTopLeftIndex = [rowStart, colStart]
          childBranch.pixelBottomRightIndex = [rowEnd, colEnd]

          childBranch.status = "remain needs to be cut"
          parentBranch.followingBranches.append(childBranch)

        # print(f"Created {len(parentBranch.followingBranches)} child patches for parent branch.")
      
    """
    QAOA image segmetation main function.

    Parameters
    ----------
    method : object (QAOAMethod)
        An input of QAOAMethod configuration.
    preContrastStreching : bool
        If true auto set sigma value.
    enhancedAdj : bool (Depreciated)
        If true use lapacian method to sharpen the image.
    flipAdjWeight : bool
        If true this application is finding max-cut,
        not min.

    Returns
    -------
    IOUScore : double
        A performance index, TP / (TP + FN + FP).
    diceCoefficient : double
        A performance index, 2 * IOUScore.
    precision : double
        A performance index, TP / (TP + FP).
    noSegmentation : bool
        Notify users QAOA can't find any negative cut.
    """
    def execute(
        self, method, preContrastStreching=False,
        enhancedAdj=False, flipAdjWeight=False
        ):
      cv2_imshow(self.image)
      # first iter works differently
      if preContrastStreching == True:
        reponse = self.contrast_stretch(self.image)
        self.image = reponse
        # sharpeningMethod = filter(self.image)
        # self.image = sharpeningMethod.laplacianImageEnhancement()
        # cv2_imshow(self.image)

      start = self.treeRoot()

      # Initial nxn patches
      self.create3x3Patches(self.image, parentBranch=start, Iter="Initial")

      adjMatrix, wMaxValueWholeImage, wMinimumValueWholeImage = mp.imageToAdj(
          start.followingBranches, numIter=1, sigma=self.sigma
          , enhancedAdj=enhancedAdj, flipAdjWeight=flipAdjWeight
      )

      # QAOA solve cut
      method.problemConfig(N=len(adjMatrix), adjMatrix=adjMatrix)
      _, _, best, numIter, bestBinary = method.executeLayerWise()
      print(bestBinary)
      gp.showCutedGraph(group=bestBinary, adjMatrix=adjMatrix, layout="grid_layout")

      # Store the patches in groups based on bestBinary
      # Determine if finished or needs to keep cutting by (most group or least group)
      # And we can directly apply mask if we know is finish
      self.updateBranchStatus(bestBinary, start, Iter="Initial")

      # Recursively cut remaining branches until condition is met
      self.keepDoingPatches(start, method=method,
        wMaxValueWholeImage=wMaxValueWholeImage,
        wMinimumValueWholeImage=wMinimumValueWholeImage,
        enhancedAdj=enhancedAdj, flipAdjWeight=flipAdjWeight
      )

      # display final result
      print("Final result:")
      cv2_imshow(self.algorithmMask)

      # Check if the mask is trivial (completely black or white)
      if np.all(self.algorithmMask == 0) or np.all(self.algorithmMask == 255):
        IOUScore = 0
        diceCoefficient = 0
        precision = 0
        noSegmentation = True
      else:
        # Perform calculation
        IOUScore = self.IOUMask()
        diceCoefficient = self.diceCoefficient()
        precision = self.precision()
        noSegmentation = False

      return IOUScore, diceCoefficient, precision, noSegmentation


    def IOUMask(self):

      # calculate IoU
      # if the same coor pixel has the same value 'True'
      intersection = np.logical_and(self.algorithmMask, self.mask)
      # if either has '1'
      union = np.logical_or(self.algorithmMask, self.mask)

      # IoU = the total '1' is the same in the coor pixel/ total pixel of '1'
      iou_score = np.sum(intersection) / np.sum(union)

      print("IOU Score:", iou_score)
      return iou_score


    def diceCoefficient(self):
          # Calculate the intersection and union of the two masks
          intersection = np.logical_and(self.algorithmMask, self.mask)
          union = np.logical_or(self.algorithmMask, self.mask)

          # Calculate the Dice coefficient
          dice = 2.0 * np.sum(intersection) / np.sum(union)

          print("Dice Coefficient:", dice)
          return dice


    def precision(self):
      # True Positive
      tp = np.sum(np.logical_and(self.algorithmMask, self.mask))

      # False Positive
      fp = np.sum(np.logical_and(self.algorithmMask, np.logical_not(self.mask)))

      # Calculate the precision
      precision = tp / (tp + fp)

      print("Precision:", precision)
      return precision


  class coordinateData:
    

    """
    TSP application solve by QAOA

    Parameters
    ----------
    maxClusterNodeSize : int
        An input data of, maximum cities QAOA quantum simulations
        that your system can handle.
        **defualt = 5**
    optimizeMethod : string
        An input data, input "layerwise" to train in layerwise method.
        **defualt = "layerwise"**
    layerwiseMethod : string
        An input data, "standard" || "interpolation".
        **defualt = "standard"**
    optimizerSelection : string
        An input data, classical optimizer selection.
        Must be supported by SciPy.
        **defualt = "COBYLA"**
    p : int
        An input data, layer used to solve this
        application.
        **defualt = 4**
    maxOptimizeItr : int,
        An input data, optimizer optimize times
        limitations.
        **defualt = 100**
    dataName : string
        An input data， solve dataset name.
        **defualt = "att532.tsp"**
    nShots : int
        An input data， amount of quantum shots.
        **defualt = 500**

    Functions
    ---------
    ->configureQAOA : Configurate how QAOA should solve the problem.
    ->configureKmeans : Configurate how many nodes in one cluster.
    ->configureDataSet : Configurate dataset name, that is being experiment.
    ->solution_total_cost : Given all node XY coordinate, and solutions route
                  dictionary. Calculate its total cost.
    ->calculate_pseudo_distance_based_on_x_y_coordinate : Calculate City to city
                                distance.
    ->draw_route_based_on_solution_data : Plotting solution graph.
    ->binary_to_decibel_path : Binary string to No. of city 
    ->executionVersion1 : A version of, using SQ-QAOA as warm solver.
                And centroid + SQ-QAOA to merge solutions.
    ->executionVersion2 : A version of, using SQ-QAOA as warm solver. 
                And use KD-tree to merge solutions.
    """
    def __init__ (
        # defualt
        self,
        maxClusterNodeSize = 5,
        optimizeMethod = "layerwise",
        layerwiseMethod = "standard",
        optimizeSelection = "COBYLA",
        p = 4,
        maxOptimizerItr = 100,
        dataName = "att532.tsp",
        nShots = 500
    ):
        self.maxClusterNodeSize = maxClusterNodeSize
        self.optimizeMethod = optimizeMethod
        self.layerwiseMethod = layerwiseMethod
        self.optimizerSelection = optimizerSelection
        self.p = p
        self.maxOptimizeItr = maxOptimizeItr
        self.dataName = dataName
        self.nShots = nShots
    

    def configureQAOA (
        self,
        optimizeMethod,
        optimizerSelection,
        maxOptimizeIter,
        p,
        nShots,
        layerwiseMethod = "standard", # optional
    ):
        """
        Configurate how QAOA should solve the problem.

        Parameters
        ----------
        optimizeMethod : string (Required)
            An input data, input "layerwise" to train in layerwise method.
        optimizerSelection : string (Required)
            An input data, classical optimizer selection.
            Must be supported by SciPy.
        layerwiseMethod : string (Required)
            An input data, "standard" || "interpolation".
        maxOptimizeItr : int (Required)
            An input data, optimizer optimize times
            limitations.
        optimizerSelection : string (Required)
            An input data, classical optimizer selection.
            Must be supported by SciPy.
        p : int (Required)
            An input data, layer used to solve this
            application.
        nShots : int (Required)
            An input data， amount of quantum shots.
        layerwiseMethod : string (Optional)
            An input data, "standard" || "interpolation".
            **defualt = "standard"**
        """
        self.optimizeMethod = optimizeMethod
        self.layerwiseMethod = layerwiseMethod
        self.optimizeSelection = optimizerSelection
        self.p = p
        self.maxOptimizeIter = maxOptimizeIter
        self.nShots = nShots


    def configureKmeans (
        self,
        maxClusterNodeSize,
    ):
        """
        Configurate how many nodes in one cluster.

        Parameters
        ----------
        maxClusterNodeSize : int (Required)
            An input data of, maximum cities QAOA quantum simulations
            that your system can handle.
            **defualt = 5**
        """
        self.maxClusterNodeSize = maxClusterNodeSize


    def configureDataSet(
        self,
        dataName
    ):    
        """
        Configurate dataset name, that is being experiment.

        Parameters
        ----------
        dataName : string (Required)
            An input data， solve dataset name.
            **defualt = "att532.tsp"**
        """
        self.dataName = dataName


    def solution_total_cost(
      self,
      tspDictionary,
      distanceType
    ) -> int:    
        """
        Given all node XY coordinate, and solutions route
        dictionary. Calculate its total cost.

        Parameters
        ----------
        tspDictionary : dict (Required)
            An input data, info about the solution.
        distanceType : string (Required)
            An input data, when distance calculation
            use which type
            Supported{"att","EUC_2D"}

        Return
        ------
        cost : int
            Route total cost.

        Example
        -------
        tspDictionary['solution'] -> [0, 1]
        tspDictionary['x'] -> [0.0, 0.0]
        tspDictionary['y'] -> [1.0, 2.0]

        # With EUC-2D as example
        cost = ceil(sqrt((
          tspDictionary['x'][0] - tspDictionary['x'][1])**2
          + tspDictionary['y'][0] - tspDictionary['y'][1])**2
        ))
        """
        cost = 0
        for eachSolution in range(len(tspDictionary[0]['solution'])):
          startCityX = tspDictionary[0]['x'][tspDictionary[0]['solution'][eachSolution]]
          startCityY = tspDictionary[0]['y'][tspDictionary[0]['solution'][eachSolution]]
          nextCityX = tspDictionary[0]['x'][tspDictionary[0]['solution'][
            (eachSolution + 1) % len(tspDictionary[0]['solution'])
          ]]
          nextCityY = tspDictionary[0]['y'][tspDictionary[0]['solution'][
            (eachSolution + 1) % len(tspDictionary[0]['solution'])
          ]]

          distanceBetweenCityToCity = self.calculate_pseudo_distance_based_on_x_y_coordinate(
            startCityX,
            nextCityX,
            startCityY,
            nextCityY,
            distanceType
          )

          cost += distanceBetweenCityToCity

        return cost


    def calculate_pseudo_distance_based_on_x_y_coordinate(
      self,
      xCoordinate, 
      nextXCoordinate, 
      yCoordinate,
      nextYCoordinate,
      weightType
    ) -> float:
      """
      Calculate City to city distance.

      Parameters
      ----------
      xCoordinate : double (Required)
          An input data, 1st city x coordinate. 
      nextXCoordinate : double (Required)
          An input data, 2st city x coordinate.
      yCoordinate : double (Required)
          An input data, 1st city y coordinate.
      nextYCoordinate : double (Required)
          An input data, 2st city y coordinate.
      weightType : string (Required)
          An input data, when distance calculation
          use which type
          Supported{"att","EUC_2D"}

      Return
      ------
      distanceBetweenNode : int
          Route cost distance.
      """
      # http://comopt.ifi.uni-heidelberg.de/software/TSPLIB95/tsp95.pdf
      if weightType == "att":
        distanceBetweenBothX = (xCoordinate - nextXCoordinate) ** 2

        distanceBetweenBothY = (yCoordinate - nextYCoordinate) ** 2

        distanceBetweenNode = np.sqrt(
          (distanceBetweenBothX + distanceBetweenBothY) / 10
        )

        integerOfDistanceBetweenNode = int(distanceBetweenNode + 0.5)

        if integerOfDistanceBetweenNode < distanceBetweenNode:
          distanceBetweenNode = integerOfDistanceBetweenNode + 1

        else:
          distanceBetweenNode = integerOfDistanceBetweenNode

        return distanceBetweenNode

      elif weightType == "euc_2d":

        distanceBetweenBothX = (xCoordinate - nextXCoordinate) ** 2

        distanceBetweenBothY = (yCoordinate - nextYCoordinate) ** 2

        distanceBetweenNode = int(np.sqrt(
          (distanceBetweenBothX + distanceBetweenBothY)
        ) + 0.5)

        return distanceBetweenNode

      else:
        raise ValueError("your weight type is not supported")


    def draw_route_based_on_solution_data(
      self,
      tspDictionary, 
      closedToInitial=False
    ) -> None:
      """
      Plotting solution graph.

      Parameters
      ----------
      tspDictionary : dict (Required)
          An input data, info about the solution.
      closedToInitial : bool (Optional)
          An input data, draw a line that return
          to initial city.
          **default = False**
      """
      colorList = [
          'b', 'g', 'r', 'c', 'm', 'y', 'tab:blue',
          'tab:orange', 'tab:green', 'tab:red',
          'tab:purple', 'tab:brown', 'tab:pink',
          'tab:gray', 'tab:olive', 'tab:cyan'
      ]
      markerList = [
        'o', 'v', '^', '<', '>', 's',
        'p', '*', 'h', 'H', 'X', 'D',
        'd', 'P', 'x'
      ]

      colorIndex = 0
      markerIndex = 0
      plt.figure(figsize=(12, 10))
      length = 0

      for groupID, groupData in tspDictionary.items():
        xCoordinate = np.array(groupData['x'])
        yCoordinate = np.array(groupData['y'])
        solution = groupData['solution']

        for i in range(
          len(solution)
          if closedToInitial == True else len(solution) - 1 # Close loop draw optinal
          ):
          currentSolutionIndex = solution[i]
          length += 1
          nextSolutionIndex = solution[
            ((i + 1) % len(solution))
            if closedToInitial == True else
            i + 1
          ]

          plt.plot(
            [xCoordinate[currentSolutionIndex], xCoordinate[nextSolutionIndex]],
            [yCoordinate[currentSolutionIndex], yCoordinate[nextSolutionIndex]],
            color=colorList[colorIndex],
            marker=markerList[markerIndex],
            markersize=5,
            linestyle='-'
          )

        if colorIndex + 1 == len(colorList):
          colorIndex = 0
          if markerIndex + 1 == len(markerList):
            print("Warning not enough verity of marker,\n reused marker and color to plot")
            markerIndex = 0
          else:
            markerIndex += 1
        else:
          colorIndex += 1

      # Plot all centroids
      for groupID, groupData in tspDictionary.items():
        if 'centroid' in groupData:
          try:
            centroid = groupData['centroid']
            plt.scatter(centroid[0], centroid[1], c='black', marker='x', s=100)
          except: # a finish solution
            break

      plt.title("All clusters TSP solutions")
      plt.xlabel("X")
      plt.ylabel("Y")
      plt.grid(True)
      plt.tight_layout()
      plt.show()


    """
    City binary string representation to decibel 
    representation.

    Parameters
    ----------
    tspDictionary : dict (Required)
        An input data, info about the solution.
    closedToInitial : bool (Optional)
        An input data, draw a line that return
        to initial city.
        **default = False**
    """
    def binary_to_decibel_path(
      self,
      binary,
      tspSize
    ):

      # Decompose solution
      decomposeString = [
        binary[i: i + tspSize - 1]
        for i in range(0, len(binary), tspSize - 1)
      ]

      # To city number
      path = [ 0 for _ in range(tspSize) ]
      for i in range(tspSize - 1):
        forAllOnes = [
          j for j, value in enumerate(decomposeString[i])
          if value == '1'
        ]
        path[i + 1] = forAllOnes[0] + 1

      return path


    """
    A version of, using SQ-QAOA as warm solver.
    And centroid + SQ-QAOA to merge solutions.

    Feedback
    --------
    Each iteration result, saved as:
    1. Initial iter : tspGroupXY_{self.dataName}.pkl
    2. Second iter : tspGroupXY1_{self.dataName}.pkl
    3. Third iter : tspGroupXY2_{self.dataName}.pkl
    .
    .
    .
    """
    def executeVersion1(self):

      # Subgraph solving should not calculate return route
      method = Method.ScipyOpimizeQAOA(
        tspApplicationCalling=True
      )

      # Setup QAOA
      method.methodConfig(
        method=self.optimizerSelection,
        minOrMax="min",
        startingWith="0"
      )

      method.circuitConfig(
        nShots=self.nShots, p=self.p,
        tspStatePreparationMethod = "", # if give known solution, it defualt to QRAM initialize circuit 
        explorerMethod = "xy-mixer"
      )

      # Sampling possible solution
      result = []
      for N in range(self.maxClusterNodeSize):
        result.append(OnceInALifeTimeTSPSamplingTree(N + 1))


      print(
        "Caution:\n"
        "We do not support any dataset that is not 6 lines after porviding\n"
        "(City index, X, Y). Please check your data.\n\n"
        "Weight type: (EUC_2D, ATT) is the only one we support.\n\n"
        "Please provide your weight type in the 5th line, using the format:\n"
        "(type:ATT) as an example.\n"
      )
      print(self.dataName)

      dataSet = open(self.dataName, "r")
      data = dataSet.readlines()
      """
      Strip remove white space and lower turn every char to lower case.
      More friendly to accept user mistakes.
      """
      weightType = data[4].split(":")[1].strip().lower()
      print(f"weightType:{weightType}")
      data = data[6: -1]
      lenOfData = len(data)
      lenClusterData = lenOfData - (lenOfData % self.maxClusterNodeSize)
      remainUnclusterData = lenOfData % self.maxClusterNodeSize
      tspData = []

      while(True):
        if os.path.exists("tspGroupXY.pkl"):
          # load previous solved data, to the latest
          with open(f"tspGroupXY.pkl", "rb") as f:
            previousInfo = pickle.load(f)

          latestIndex = 0 # store the latest pkl index
          tryIndex = 0 # increment trial index
          while(True):
            try:
              tryIndex += 1
              with open(f"tspGroupXY{tryIndex}.pkl", "rb") as f:
                previousInfo = pickle.load(f)

              # if fail to load (to the latest already), would crash.
              latestIndex = tryIndex
            except:
              break
          
          # Checking if final iteration,  neccessary for K-means constrained
          if len(previousInfo.keys()) > 5:

            # Calculation of cluster size needed, for K-means constrained and left overs data points.
            previousAmountOfCluster = len(previousInfo)
            numberOfCluster = previousAmountOfCluster // self.maxClusterNodeSize
            readyToClusterCentroid = (numberOfCluster) * self.maxClusterNodeSize
            cantExactlySatisfyMaxNode = previousAmountOfCluster - readyToClusterCentroid

            """
            Directly selecting end of the list by cantExactlySatisfyMaxNode amount of element,
            Left overs data points might be to far away from each other.

            Basic resolve idea:
            Nearest neighour to the last index data point.
            """
           
            if cantExactlySatisfyMaxNode > 1:

              # elitism storing best value and their index
              topSmallestDistanceToAbandonedGroupCost = [
                float("inf") for _ in range(cantExactlySatisfyMaxNode - 1)
              ]
              topSmallestDistanceToAbandonedGroupIndex = [
                0 for _ in range(cantExactlySatisfyMaxNode - 1)
              ]

              for eachGroup in list(previousInfo)[:-1]:

                distanceFromAbandonedGroupToOthers = self.calculate_pseudo_distance_based_on_x_y_coordinate(
                  previousInfo[previousAmountOfCluster - 1]['centroid'][0],
                  previousInfo[eachGroup]['centroid'][0],
                  previousInfo[previousAmountOfCluster - 1]['centroid'][1],
                  previousInfo[eachGroup]['centroid'][1],
                  weightType
                )

                i = 0
                while(
                    topSmallestDistanceToAbandonedGroupCost[i] >
                    distanceFromAbandonedGroupToOthers
                  ):
                  topSmallestDistanceToAbandonedGroupCost[i] = distanceFromAbandonedGroupToOthers
                  topSmallestDistanceToAbandonedGroupIndex[i] = eachGroup

              # Swapping group, so uncluster could be closest
              for indexCounting, index in enumerate(
                  topSmallestDistanceToAbandonedGroupIndex
                ):
                previousInfo[index], previousInfo[
                  readyToClusterCentroid + indexCounting
                ] = previousInfo[readyToClusterCentroid + indexCounting], previousInfo[index]

            # Get all group centroid x,y
            clusterData = list(
              map(
                lambda groupLabel:
                previousInfo[groupLabel]["centroid"],
                previousInfo
              )
            )

            """
            ClusterData - The size of this list data points, is statisfy max cluster node size user input.
            UnclusterData - Who has less max cluster node data points.

            Basic idea:
            Since we cant put all data at once to cluster, saperation clustering.
            ClusterData -> size_max = self.maxNodeInCluster, multiple.
            UnclusterData -> size_max = len(UnclusterData), 1 group.
            """
            unclusterData = clusterData[readyToClusterCentroid:]
            clusterData = clusterData[:readyToClusterCentroid]

            clf = KMeansConstrained(
                  n_clusters=numberOfCluster,
                  size_min=self.maxClusterNodeSize,
                  size_max=self.maxClusterNodeSize,
                  random_state=0
                )

            clf.fit_predict(np.array(clusterData))

            jointGroupContains = {}

            # Enormous plot required combinations
            colorList = [
              'b', 'g', 'r', 'c', 'm', 'y', 'tab:blue',
              'tab:orange', 'tab:green', 'tab:red',
              'tab:purple', 'tab:brown', 'tab:pink',
              'tab:gray', 'tab:cyan'
            ]

            markerList = [
              'o', 'v', '^', '<', '>', 's',
              'p', '*', 'h', 'H', 'X', 'D',
              'd', 'P', 'x'
            ]

            colorIndex = 0
            markerIndex = 0

            for groupLabel in range(numberOfCluster):

              # Initiallize data description of each group
              jointGroupContains[groupLabel] = {
                "x": [],
                "y": [],
                "solution": [],
                "cost": 0,
                "groupCentroidX": [],
                "groupCentroidY": []
              }
              eachGroupDataX = []
              eachGroupDataY = []

              for indexOfCentroid in range(readyToClusterCentroid):

                # Select same group data points
                if clf.labels_[indexOfCentroid] == groupLabel:
                  eachGroupDataX.append(clusterData[indexOfCentroid][0])
                  eachGroupDataY.append(clusterData[indexOfCentroid][1])
                  jointGroupContains[groupLabel] = {
                    # Merge coordinate
                    "x": jointGroupContains[groupLabel]["x"] +
                        [ previousInfo[indexOfCentroid]["x"] ],
                    "y": jointGroupContains[groupLabel]["y"] +
                        [ previousInfo[indexOfCentroid]["y"] ],
                    # Relocated solution represented X Y coordiante
                    "solution": jointGroupContains[groupLabel]["solution"] +
                          [ previousInfo[indexOfCentroid]["solution"] ],
                    # Associated group total cost
                    "cost": jointGroupContains[groupLabel]["cost"] +
                          previousInfo[indexOfCentroid]["cost"],
                  }

              # For QAOA solve
              jointGroupContains[groupLabel]["groupCentroidX"] = eachGroupDataX
              jointGroupContains[groupLabel]["groupCentroidY"] = eachGroupDataY

              # Each group cluster result centroid coordinate, for next iteration use.
              jointGroupContains[groupLabel]["centroid"] = clf.cluster_centers_[groupLabel]

              # Plot city centroid points
              plt.scatter(
                eachGroupDataX,
                eachGroupDataY,
                color=colorList[colorIndex],
                marker=markerList[markerIndex],
                s=25
              )

              if colorIndex + 1 == len(colorList):
                colorIndex = 0
                if markerIndex + 1 == len(markerList):
                  print("Warning not enough verity of marker,\n reused marker and color to plot")
                  markerIndex = 0
                else:
                  markerIndex += 1
              else:
                colorIndex += 1

            if cantExactlySatisfyMaxNode > 0:
              clf = KMeansConstrained(
                    n_clusters=1,
                    size_min=len(unclusterData),
                    size_max=len(unclusterData),
                    random_state=0
                  )

              clf.fit_predict(np.array(unclusterData))

              jointGroupContains[numberOfCluster] = {
                "x": [],
                "y": [],
                "solution": [],
                "cost": 0
              }

              eachGroupDataX = []
              eachGroupDataY = []
              for indexOfUnclusterData in range(len(unclusterData)):
                unclusterGroupLabel = readyToClusterCentroid + indexOfUnclusterData
                eachGroupDataX.append(unclusterData[indexOfUnclusterData][0])
                eachGroupDataY.append(unclusterData[indexOfUnclusterData][1])
                jointGroupContains[numberOfCluster] = {
                  "x": jointGroupContains[numberOfCluster]["x"] +
                    [ previousInfo[unclusterGroupLabel]["x"] ],
                  "y": jointGroupContains[numberOfCluster]["y"] +
                    [ previousInfo[unclusterGroupLabel]["y"] ],
                  "solution": jointGroupContains[numberOfCluster]["solution"] +
                      [ previousInfo[unclusterGroupLabel]["solution"] ],
                  "cost": jointGroupContains[numberOfCluster]["cost"] +
                      previousInfo[unclusterGroupLabel]["cost"],
                }

              jointGroupContains[numberOfCluster]["groupCentroidX"] = eachGroupDataX
              jointGroupContains[numberOfCluster]["groupCentroidY"] = eachGroupDataY
              jointGroupContains[numberOfCluster]["centroid"] = clf.cluster_centers_[0]

              plt.scatter(
                eachGroupDataX,
                eachGroupDataY,
                color=colorList[colorIndex],
                marker=markerList[markerIndex],
                s=25
              )

            plt.title("Centroid to TSP graph using k-means-constrained for group joining")
            plt.xlabel("X")
            plt.ylabel("Y")
            plt.grid(True)
            plt.tight_layout()
            plt.show()
            print(len(jointGroupContains))
            print(jointGroupContains.keys())

          else: # small enough to ending Join process by SQ-QAOA solving

            if len(previousInfo.keys()) > 2:

              # Fix: fixed starting city issues
              indexSortedAccordingToCentroidX = sorted(
                range(len(previousInfo.keys())),
                key=lambda eachElement:
                previousInfo[eachElement]["centroid"][0]
              )

              highestXValue = previousInfo[indexSortedAccordingToCentroidX[0]]["centroid"][0]
              sameXCount = 1

              for i in range(1, len(indexSortedAccordingToCentroidX)):
                idx = indexSortedAccordingToCentroidX[i]
                if previousInfo[idx]["centroid"][0] == highestXValue:
                  sameXCount += 1
                else:
                  break

              topXIndex = indexSortedAccordingToCentroidX[:sameXCount]
              highestXsortedByYIndex = sorted(
                topXIndex,
                key=lambda eachIndex:
                previousInfo[eachIndex]["centroid"][1]
              )

              indexSortedAccordingToCentroidX[:sameXCount] = highestXsortedByYIndex

              # Backup
              backUpDictionary = [
                previousInfo[eachIndex]["centroid"]
                for eachIndex in indexSortedAccordingToCentroidX
              ]

              for eachElementIndex in range(len(backUpDictionary)):
                jointGroupContains[eachElementIndex][
                  "groupCentroidX"
                ] = backUpDictionary[eachElementIndex]

              adjMatrix = np.zeros((
                len(previousInfo.keys()),
                len(previousInfo.keys())
              ))

              for i in range(len(previousInfo.keys())):
                for j in range(i + 1, len(previousInfo.keys())):
                  distanceCentroidIJ = self.calculate_pseudo_distance_based_on_x_y_coordinate(
                      previousInfo[i]["centroid"][0],
                      previousInfo[j]["centroid"][0],
                      previousInfo[i]["centroid"][1],
                      previousInfo[j]["centroid"][1],
                      weightType
                  )

                  # Symmetry TSP
                  adjMatrix[i][j] = distanceCentroidIJ
                  adjMatrix[j][i] = distanceCentroidIJ

              N = len(adjMatrix)
              method.statePrepConfig(knownSolution=result[N - 2])
              method.problemConfig(
                N=N,
                adjMatrix=adjMatrix,
                problemType='tsp'
              )

              if self.optimizeMethod == "layerwise":
                _, _, _, _, tspBestBinary = method.executeLayerWise(self.layerwiseMethod)
              else: # default all layer at once
                _, _, _, _, tspBestBinary = method.execute()

              path = self.binary_to_decibel_path(tspBestBinary, N)

            else:
              path = [ groupID for groupID in range(len(previousInfo.keys())) ]
            
            """
            Nearest Neighbor iter 1 evaluate: Group 1 ready to joint
            solution starting city and group 2 starting city,
            who is in degree 1.

            Nearest Neighbor iter 2 evaluate: g1 starting city and g2 ending

            Nearest Neighbor iter 3 evaluate: g1 ending city and g2 starting

            Nearest Neighbor iter 4 evaluate: g1 ending city and g2 ending

            Next solution group g2 to g3:
            g2 has only one entry point left, 2 combination of links
            """


            everyJointAction = [
              [None, None] for _ in range(len(path) - 1)
            ]

            actionCost = [
              float("inf") for _ in range(len(path) - 1)
            ]

            groupDictionary = {}

            for pathIndex in range(len(path) - 1):

              # Calculate cost to degree one node first joint group and second
              firstGroupSolution = previousInfo[path[pathIndex]]["solution"]
              secondGroupSolution = previousInfo[path[pathIndex + 1]]["solution"]

              if pathIndex == 0:
                numberOfLinkCombinations = 4
              else:
                numberOfLinkCombinations = 2

              for combinationsLinks in range(numberOfLinkCombinations):

                if numberOfLinkCombinations != 2:
                  groupOneAction = 0 if combinationsLinks // 2 == 0 else -1
                  groupSecondAction = 0 if combinationsLinks % 2 == 0 else -1
                else: # linked group only have one node degree one it is fixed
                  groupOneAction = 0 if everyJointAction[pathIndex - 1][1] == -1 else -1
                  groupSecondAction = 0 if combinationsLinks % 2 == 0 else -1

                distanceBetweenDegree1Node = self.calculate_pseudo_distance_based_on_x_y_coordinate(
                  previousInfo[path[pathIndex]]["x"][firstGroupSolution[groupOneAction]],
                  previousInfo[path[pathIndex + 1]]["x"][secondGroupSolution[groupSecondAction]],
                  previousInfo[path[pathIndex]]["y"][firstGroupSolution[groupOneAction]],
                  previousInfo[path[pathIndex + 1]]["y"][secondGroupSolution[groupSecondAction]],
                  weightType
                )

                if actionCost[pathIndex] > distanceBetweenDegree1Node:
                  everyJointAction[pathIndex] = [groupOneAction, groupSecondAction]
                  actionCost[pathIndex] = distanceBetweenDegree1Node

            """
            solution 1 -> [0, 1, 2, 3] corresponding XY -> [....],
            solution 2 -> [0, 3, 1, 2] corresponding XY -> [....].
            If Joint action taken 0, -1 (start solution 1 joint end solution 2):
            reverse solution 1 solution string,
            reverse solution 2 solution string, and add each solution city number by
            solution 1 data length.
            And joint array, added action cost to total cost.
            Example output [0, 1, 2, 3, 6, 5, 7, 4], merged XY=[....]
            """
            if len(previousInfo.keys()) != 1: # perfect grouped to 1 already 
              if everyJointAction[0][0] == 0:
                newSolution = list(reversed(
                  previousInfo[path[0]]["solution"]
                ))
              else:
                newSolution = previousInfo[path[0]]["solution"]

              newSolutionCityX = previousInfo[path[0]]["x"]
              newSolutionCityY = previousInfo[path[0]]["y"]

              for pathIndex in range(1, len(path)):
                if everyJointAction[pathIndex - 1][1] == -1:
                  nextJointSolution = list(reversed(
                    previousInfo[path[pathIndex]]["solution"]
                  ))
                else:
                  nextJointSolution = previousInfo[path[pathIndex]]["solution"]


                # Reallocate coresspond XY to solution
                nextJointSolution = [
                  len(newSolution) + eachCity
                  for eachCity in nextJointSolution
                ]

                nextJointSolutionX = previousInfo[path[pathIndex]]["x"]
                nextJointSolutionY = previousInfo[path[pathIndex]]["y"]


                # Merge solution
                newSolution = np.concatenate([newSolution, nextJointSolution])
                newSolutionCityX = np.concatenate([newSolutionCityX, nextJointSolutionX])
                newSolutionCityY = np.concatenate([newSolutionCityY, nextJointSolutionY])
                # add joint cost

                newSolution = list(newSolution)
                newSolutionCityX = newSolutionCityX.tolist()
                newSolutionCityY = newSolutionCityY.tolist()

              groupDictionary[0] = {
                "solution": newSolution,
                "x": newSolutionCityX,
                "y": newSolutionCityY,
                "cost": self.solution_total_cost,
                "centroid": None
              }
            
            else:

              groupDictionary = previousInfo

            print(f"Final: {groupDictionary[0]['solution']}")

            # Save merge result
            with open(f'tspGroupXY{latestIndex + 1}_{self.dataName}.pkl', 'wb') as f:
              pickle.dump(groupDictionary, f)

            self.draw_route_based_on_solution_data(groupDictionary, True)
            print(self.solution_total_cost(tspDictionary=groupDictionary, distanceType=weightType))

            break


          for eachGroup in jointGroupContains:

            if len(jointGroupContains[eachGroup]['solution']) > 2: # Decision on it is too small of a problem.

              # Fix: fixed starting city issues
              indexSortedAccordingToX = sorted(
                range(len(jointGroupContains[eachGroup]["groupCentroidX"])),
                key=lambda eachElement:
                jointGroupContains[eachGroup]["groupCentroidX"][eachElement]
              )

              highestXValue = jointGroupContains[eachGroup]["groupCentroidX"][indexSortedAccordingToX[0]]
              sameXCount = 1

              for i in range(1, len(indexSortedAccordingToX)):
                idx = indexSortedAccordingToX[i]
                if jointGroupContains[eachGroup]["groupCentroidX"][idx] == highestXValue:
                  sameXCount += 1
                else:
                  break

              topXIndex = indexSortedAccordingToX[:sameXCount]
              highestXsortedByYIndex = sorted(
                topXIndex,
                key=lambda eachIndex:
                jointGroupContains[eachGroup]["groupCentroidY"][eachIndex]
              )

              indexSortedAccordingToX[:sameXCount] = highestXsortedByYIndex

              jointGroupContains[eachGroup]["groupCentroidX"] = [
                jointGroupContains[eachGroup]["groupCentroidX"][index]
                for index in indexSortedAccordingToX
              ]
              # Reallocate original pose
              jointGroupContains[eachGroup]["groupCentroidY"] = [
                jointGroupContains[eachGroup]["groupCentroidY"][index]
                for index in indexSortedAccordingToX
              ]

              jointGroupContains[eachGroup]["solution"] = [
                jointGroupContains[eachGroup]["solution"][index]
                for index in indexSortedAccordingToX
              ]

              jointGroupContains[eachGroup]["x"] = [
                jointGroupContains[eachGroup]["x"][index]
                for index in indexSortedAccordingToX
              ]

              jointGroupContains[eachGroup]["y"] = [
                jointGroupContains[eachGroup]["y"][index]
                for index in indexSortedAccordingToX
              ]

              adjMatrix = np.zeros((
                len(jointGroupContains[eachGroup]["groupCentroidX"]),
                len(jointGroupContains[eachGroup]["groupCentroidX"])
              ))

              for i in range(len(jointGroupContains[eachGroup]["groupCentroidX"])):
                for j in range(i + 1, len(jointGroupContains[eachGroup]["groupCentroidX"])):
                  # Pseudo-Euclidean distance
                  # Ref:https://github.com/bcamath-ds/OPLib/blob/master/instances/README.md

                  distanceCentroidIJ = self.calculate_pseudo_distance_based_on_x_y_coordinate(
                    jointGroupContains[eachGroup]["groupCentroidX"][i],
                    jointGroupContains[eachGroup]["groupCentroidX"][j],
                    jointGroupContains[eachGroup]["groupCentroidY"][i],
                    jointGroupContains[eachGroup]["groupCentroidY"][j],
                    weightType
                  )

                  # Symmetry TSP
                  adjMatrix[i][j] = distanceCentroidIJ
                  adjMatrix[j][i] = distanceCentroidIJ

              N = len(adjMatrix)

              method.statePrepConfig(knownSolution=result[N - 2])
              method.problemConfig(
                N=N,
                adjMatrix=adjMatrix,
                problemType='tsp'
              )

              if self.optimizeMethod == "layerwise":
                _, _, _, _, tspBestBinary = method.executeLayerWise(self.layerwiseMethod)
              else: # default all layer at once
                _, _, _, _, tspBestBinary = method.execute()

              path = self.binary_to_decibel_path(tspBestBinary, N)

            else: # too small

              path = list(range(len(jointGroupContains[eachGroup]['solution'])))

            if len(jointGroupContains[eachGroup]['solution']) > 1:

              print(f"Group{eachGroup} solution:{path}")
              print("Joint solution process....")

              """
                Nearest Neighbour decision calculate joint solution degree=1 node
                (start index and last index city), both linked group best linked
                decision.
              """

              everyJointAction = [
                [None, None] for _ in range(len(path) - 1)
              ]

              actionCost = [
                float("inf") for _ in range(len(path) - 1)
              ]

              for pathIndex in range(len(path) - 1):
                # Calculate cost to degree one node first joint group and second
                firstGroupSolution = jointGroupContains[eachGroup]["solution"][
                    path[pathIndex]
                ]
                secondGroupSolution = jointGroupContains[eachGroup]["solution"][
                    path[pathIndex + 1]
                ]


                """
                Nearest Neighbor iter 1 evaluate: Group 1 ready to joint
                solution starting city and group 2 starting city,
                who is in degree 1.

                Nearest Neighbor iter 2 evaluate: g1 starting city and g2 ending

                Nearest Neighbor iter 3 evaluate: g1 ending city and g2 starting

                Nearest Neighbor iter 4 evaluate: g1 ending city and g2 ending
                """
                if pathIndex == 0:
                  numberOfLinkCombinations = 4

                else:
                  numberOfLinkCombinations = 2

                for combinationsLinks in range(numberOfLinkCombinations):

                  if numberOfLinkCombinations != 2:
                    groupOneAction = 0 if combinationsLinks // 2 == 0 else -1
                    groupSecondAction = 0 if combinationsLinks % 2 == 0 else -1
                  else: # linked group only have one node degree one it is fixed
                    groupOneAction = 0 if everyJointAction[pathIndex - 1][1] == -1 else -1
                    groupSecondAction = 0 if combinationsLinks % 2 == 0 else -1

                  distanceBetweenDegree1Node = self.calculate_pseudo_distance_based_on_x_y_coordinate(
                    jointGroupContains[eachGroup]["x"][path[pathIndex]][firstGroupSolution[groupOneAction]],
                    jointGroupContains[eachGroup]["x"][path[pathIndex + 1]][secondGroupSolution[groupSecondAction]],
                    jointGroupContains[eachGroup]["y"][path[pathIndex]][firstGroupSolution[groupOneAction]],
                    jointGroupContains[eachGroup]["y"][path[pathIndex + 1]][secondGroupSolution[groupSecondAction]],
                    weightType
                  )

                  print(f"combination: {numberOfLinkCombinations} \n action:{groupOneAction,groupSecondAction}: \n actionCost:{distanceBetweenDegree1Node}")
                  if actionCost[pathIndex] > distanceBetweenDegree1Node:
                    everyJointAction[pathIndex] = [groupOneAction, groupSecondAction]
                    actionCost[pathIndex] = distanceBetweenDegree1Node
              print(f"Final: {everyJointAction}")

              """
              solution 1 -> [0, 1, 2, 3] corresponding XY -> [....],
              solution 2 -> [0, 3, 1, 2] corresponding XY -> [....].
              If Joint action taken 0, -1 (start solution 1 joint end solution 2):
              reverse solution 1 solution string,
              reverse solution 2 solution string, and add each solution city number by
              solution 1 data length.
              And joint array, added action cost to total cost.
              Example output [0, 1, 2, 3, 6, 5, 7, 4], merged XY=[....]
              """

              newGroupCost = jointGroupContains[eachGroup]["cost"]

              if everyJointAction[0][0] == 0:
                newSolution = list(reversed(
                  jointGroupContains[eachGroup]["solution"][path[0]]
                ))
              else:
                newSolution = jointGroupContains[eachGroup]["solution"][path[0]]

              newSolutionCityX = jointGroupContains[eachGroup]["x"][path[0]]
              newSolutionCityY = jointGroupContains[eachGroup]["y"][path[0]]

              for pathIndex in range(1, len(path)):

                if everyJointAction[pathIndex - 1][1] == -1:
                  nextJointSolution = list(reversed(
                    jointGroupContains[eachGroup]["solution"][path[pathIndex]]
                  ))
                else:
                  nextJointSolution = jointGroupContains[eachGroup]["solution"][path[pathIndex]]


                # Reallocate coresspond XY to solution
                nextJointSolution = [
                  len(newSolution) + eachCity
                  for eachCity in nextJointSolution
                ]

                nextJointSolutionX = jointGroupContains[eachGroup]["x"][path[pathIndex]]
                nextJointSolutionY = jointGroupContains[eachGroup]["y"][path[pathIndex]]


                # Merge solution
                newSolution = np.concatenate([newSolution, nextJointSolution])
                newSolutionCityX = np.concatenate([newSolutionCityX, nextJointSolutionX])
                newSolutionCityY = np.concatenate([newSolutionCityY, nextJointSolutionY])
                # add joint cost
                newGroupCost += actionCost[pathIndex - 1]

              print(f"Group{eachGroup} Joint to {newSolution}")
              # Update after merges
              jointGroupContains[eachGroup]["solution"] = newSolution
              jointGroupContains[eachGroup]["x"] = newSolutionCityX
              jointGroupContains[eachGroup]["y"] = newSolutionCityY
              jointGroupContains[eachGroup]["cost"] = newGroupCost

            else:
              print(f"Group{eachGroup} Joint to {path}")
              jointGroupContains[eachGroup]["solution"] = jointGroupContains[eachGroup]["solution"][0]
              jointGroupContains[eachGroup]["x"] = jointGroupContains[eachGroup]["x"][0]
              jointGroupContains[eachGroup]["y"] = jointGroupContains[eachGroup]["y"][0]
              jointGroupContains[eachGroup]["cost"] = 0

          with open(f'tspGroupXY{latestIndex + 1}_{self.dataName}.pkl', 'wb') as f:
            pickle.dump(jointGroupContains, f)

          print(f"Saved tspGroupXY{latestIndex + 1}.pkl as {latestIndex + 1} loop solve.")

        else:
          # print("No")
          """
          First iteration cluster based on
          dataset length.
          """
          for i, eachLine in enumerate(data):
            data[i] = eachLine.replace("\n", "")
            splitData = eachLine.split(" ")
            while True:
              try:
                  splitData.remove("")
              except:
                break
            # city = splitData[0]
            x = float(splitData[1])
            y = float(splitData[2])
            tspData.append([x, y])

          indexSortedAccordingToX = sorted(
            range(len(tspData)),
            key=lambda eachElement: tspData[eachElement][0]
          )

          highestXValue = tspData[indexSortedAccordingToX[0]][0]
          sameXCount = 1

          for i in range(1, len(indexSortedAccordingToX)):
            idx = indexSortedAccordingToX[i]
            if tspData[idx][0] == highestXValue:
              sameXCount += 1
            else:
              break

          topXIndex = indexSortedAccordingToX[:sameXCount]
          highestXsortedByYIndex = sorted(
            topXIndex,
            key=lambda eachIndex:
            tspData[eachIndex][1]
          )
          indexSortedAccordingToX[:sameXCount] = highestXsortedByYIndex

          tspData = [
            tspData[index]
            for index in indexSortedAccordingToX
          ]

          unclusterData = tspData[lenClusterData:]
          tspData = tspData[:lenClusterData]

          clf = KMeansConstrained(
              n_clusters=lenClusterData // self.maxClusterNodeSize,
              size_min=self.maxClusterNodeSize,
              size_max=self.maxClusterNodeSize,
              random_state=0
          )
          clf.fit_predict(np.array(tspData))

          colorList = [
            'b', 'g', 'r', 'c', 'm', 'y', 'tab:blue',
            'tab:orange', 'tab:green', 'tab:red',
            'tab:purple', 'tab:brown', 'tab:pink',
            'tab:gray', 'tab:olive', 'tab:cyan'
          ]
          markerList = [
            'o', 'v', '^', '<', '>', 's',
            'p', '*', 'h', 'H', 'X', 'D',
            'd', 'P', 'x'
          ]

          tspNpArray = np.array(tspData)
          uniqueGroupLabels = list(range(0, lenClusterData // self.maxClusterNodeSize))

          if len(uniqueGroupLabels) > len(markerList) * len(colorList):
            raise Exception("Not enough marker and color combinations for all clusters")

          plt.figure(figsize=(10, 8))

          # dictionary for easier time
          tspGroupXY = {}

          # Plot group
          markerIndex = 0
          colorIndex = 0
          for clusterLabels in uniqueGroupLabels:

            # Each group data
            clusterIncludedData = tspNpArray[clf.labels_ == clusterLabels]

            tspGroupXY[clusterLabels] = {
                'x': clusterIncludedData[:, 0],
                'y': clusterIncludedData[:, 1],
                'centroid': clf.cluster_centers_[clusterLabels]
            }

            marker = markerList[markerIndex]
            color = colorList[colorIndex]

            plt.scatter(
              clusterIncludedData[:, 0], 
              clusterIncludedData[:, 1], 
              marker=marker, 
              color=color,
              label=f"Cluster {clusterLabels}", 
              s=25
            )

            if markerIndex == len(markerList) - 1:
              markerIndex = 0
              colorIndex += 1
            else:
              markerIndex += 1

          # Last label plot
          if remainUnclusterData != 0:
            labelOfTheGroup = lenClusterData // self.maxClusterNodeSize
            for eachIndex in range(len(unclusterData)):
              x = float(unclusterData[eachIndex][0])
              y = float(unclusterData[eachIndex][1])
              tspData.append([x, y])

            clf = KMeansConstrained(
              n_clusters=1,
              size_min=remainUnclusterData,
              size_max=remainUnclusterData,
              random_state=0
            )
            clf.fit_predict(np.array(tspData[lenOfData - remainUnclusterData:]))

            clusterIncludedData = np.array(tspData[lenOfData - remainUnclusterData:])

            tspGroupXY[labelOfTheGroup] = {
                'x': clusterIncludedData[:, 0],
                'y': clusterIncludedData[:, 1],
                'centroid': clf.cluster_centers_[0]
            }

            uniqueGroupLabels.append(labelOfTheGroup)

            marker = markerList[markerIndex]
            color = colorList[colorIndex]

            plt.scatter(
              clusterIncludedData[:,0], 
              clusterIncludedData[:,1], 
              marker=marker, 
              color=color, 
              label=f"Cluster {labelOfTheGroup}",
              s=25
            )

          plt.title("Preprocessed TSP graph to subgraph using k-means-constrained")
          plt.xlabel("X")
          plt.ylabel("Y")
          plt.grid(True)
          plt.tight_layout()
          plt.show()

          for eachGroup in tspGroupXY:

            # Fix: fixed starting city issues
            indexSortedAccordingToX = sorted(
              range(len(tspGroupXY[eachGroup]['x'])),
              key=lambda eachElement: tspGroupXY[eachGroup]['x'][eachElement]
            )

            highestXValue = tspGroupXY[eachGroup]['x'][indexSortedAccordingToX[0]]
            sameXCount = 1

            for i in range(1, len(indexSortedAccordingToX)):
              idx = indexSortedAccordingToX[i]
              if tspGroupXY[eachGroup]['x'][idx] == highestXValue:
                sameXCount += 1
              else:
                break

            topXIndex = indexSortedAccordingToX[:sameXCount]
            highestXsortedByYIndex = sorted(
              topXIndex,
              key=lambda eachIndex:
              tspGroupXY[eachGroup]['y'][eachIndex]
            )

            indexSortedAccordingToX[:sameXCount] = highestXsortedByYIndex

            tspGroupXY[eachGroup]['x'] = [
              tspGroupXY[eachGroup]['x'][index]
              for index in indexSortedAccordingToX
            ]
            # Reallocate original pose
            tspGroupXY[eachGroup]['y'] = [
              tspGroupXY[eachGroup]['y'][index]
              for index in indexSortedAccordingToX
            ]

            adjMatrix = np.zeros((
              len(tspGroupXY[eachGroup]['x']),
              len(tspGroupXY[eachGroup]['x'])
            ))

            for i in range(len(tspGroupXY[eachGroup]['x'])):
              for j in range(i + 1, len(tspGroupXY[eachGroup]['x'])):

                distanceIJ = self.calculate_pseudo_distance_based_on_x_y_coordinate(
                  tspGroupXY[eachGroup]['x'][i], tspGroupXY[eachGroup]['x'][j],
                  tspGroupXY[eachGroup]['y'][i], tspGroupXY[eachGroup]['y'][j],
                  weightType
                )

                adjMatrix[i][j] = distanceIJ
                adjMatrix[j][i] = distanceIJ

            N = len(adjMatrix)

            if N > 2:
              method.statePrepConfig(knownSolution=result[N-2])
              method.problemConfig(
                N=N,
                adjMatrix=adjMatrix,
                problemType='tsp'
              )

              if self.optimizeMethod == "layerwise":
                _, _, _, iterT, tspBestBinary = method.executeLayerWise(self.layerwiseMethod)
              else: # default all layer at once
                _, _, _, iterT, tspBestBinary = method.execute()

              # Decompose solution
              decomposeString = [
                tspBestBinary[i: i + N - 1]
                for i in range(0, len(tspBestBinary), N - 1)
              ]

              # To city number
              path = [ 0 for _ in range(N) ]
              for i in range(N - 1):
                forAllOnes = [j for j, value in enumerate(decomposeString[i]) if value == '1']
                path[i + 1] = forAllOnes[0] + 1

              print(f"Group{eachGroup} solution:{path}")

            else:

              path = list(range(N))
              print(f"Group{eachGroup} solution:{path}")
            best = 0
            """
            doesn't matter will recalculate when whole solution
            is QAOA OBJ only has 2-D eud version cost
            """

            tspGroupXY[eachGroup]['solution'] = path
            tspGroupXY[eachGroup]['cost'] = best

          # Save
          with open(f'tspGroupXY_{self.dataName}.pkl', 'wb') as f:
            pickle.dump(tspGroupXY, f)
          
          print(f"Saved tspGroupXY_{self.dataName}.pkl as first loop solve.")

          self.draw_route_based_on_solution_data(tspGroupXY)


    """
    A version of, using SQ-QAOA as warm solver. 
    And use KD-tree to merge solutions.

    Feedback
    --------
    Save each iteration result as:
    1. Initial iter : tspGroupXY_{self.dataName}_KDTree.pkl
    2. Second iter : tspGroupXY2_{self.dataName}_KDtree.pkl
    """
    def executeVersion2(self):

      method = Method.ScipyOpimizeQAOA(
        tspApplicationCalling=True
      )

      method.methodConfig(
        method=self.optimizerSelection, 
        minOrMax="min", 
        startingWith="0"
      )

      result = []
      for N in range(self.maxClusterNodeSize):
        result.append(OnceInALifeTimeTSPSamplingTree(N + 1))

      method.circuitConfig(
        nShots=self.nShots, p=self.p,
        tspStatePreparationMethod = "",
        explorerMethod = "xy-mixer"
      )

      print(
        "Caution:\n"
        "We do not support any dataset that is not 6 lines after porviding\n"
        "(City index, X, Y). Please check your data.\n\n"
        "Weight type: (EUC_2D, ATT) is the only one we support.\n\n"
        "Please provide your weight type in the 5th line, using the format:\n"
        "(type:ATT) as an example.\n"
      )

      dataSet = open(self.dataName, "r")
      data = dataSet.readlines()

      """
      Strip remove white space and lower turn every char to lower case.
      More friendly to accept user mistakes.
      """
      weightType = data[4].split(":")[1].strip().lower()
      print(f"weightType:{weightType}")
      data = data[6: -1]
      lenOfData = len(data)
      lenClusterData = lenOfData - (lenOfData % self.maxClusterNodeSize)
      remainUnclusterData = lenOfData % self.maxClusterNodeSize
      tspData = []

      for step in range(1, 2 + 1): # two steps TSP

        if step == 2:

          with open(f"tspGroupXY_{self.dataName}_KDTree.pkl", "rb") as f:
            previousInfo = pickle.load(f)

          dataPoints = []
          representLabel = []

          for eachGroup in previousInfo:
            """
            Define the data for KDTree to perform nearest neighbor,
            to make an decision, how to join subgroup.
            """
            # subgraph solution degree 1 node
            startPointSolutionIndex = previousInfo[eachGroup]['solution'][0]
            endPointSolutionIndex = previousInfo[eachGroup]['solution'][-1]

            # group start point
            startPointX = previousInfo[eachGroup]['x'][startPointSolutionIndex]
            startPointY = previousInfo[eachGroup]['y'][startPointSolutionIndex]

            # group end point
            endPointX = previousInfo[eachGroup]['x'][endPointSolutionIndex]
            endPointY = previousInfo[eachGroup]['y'][endPointSolutionIndex]

            dataPoints.append([startPointX, startPointY])
            dataPoints.append([endPointX, endPointY])

            # label data
            representLabel.append([eachGroup, 'start'])
            representLabel.append([eachGroup, 'end'])

          # for falsely joining, already connected solution
          visitedList = []

          # Build KDTree
          tree = KDTree(dataPoints)

          """
          Basic idea: "Continuous joining list"
          visited structure sequence = merge sequence
          random subtour 'start' entry point as first point,
          KD-Tree search next best entry point exluding
          joining to 0 subtour other entry point. (illegal move)
          Example if KD-Tree said "nearest is 1 group start point",
          we have visited [[0, 'start'], [1, 'start']] take random
          point excluding visited the list will possibily resulting:
          [[0, 'start'], [1, 'start'], [4, 'end'], [6, 'start']]
          no connection between [0, 'start'], [1, 'start'] -> ...
          making implementation hard.

          So, the idea is we should countinue on searching subtour 1
          contrary entry point.
          Visited list then will looked like this, if group one nearest
          is subtour 6 start:
          [[0, 'start'], [1, 'start'], [1, 'end'], [6, 'start']]
          countinue on group 6 contrary point until all point visited.
          """
          referenceStartGroup = random.randint(1, len(previousInfo) - 1)
          referenceStartGroupLinkEntryType = 'start'
          visitedList.append([referenceStartGroup, referenceStartGroupLinkEntryType])
          x = previousInfo[referenceStartGroup]['x'][0]
          y = previousInfo[referenceStartGroup]['y'][0]
          data = [x, y]

          # Querying KDtree to get nearest coordinate data point.
          _, rankIndex = tree.query([data], k=len(dataPoints))
          rankIndex = rankIndex[0]
 
          for eachIndex in rankIndex:
            # Condition if same group (invalid)
            if referenceStartGroup == representLabel[eachIndex][0]:
              continue
            # Valid, note group and type of entry point for next contrary type point searching.
            latestGroupID = representLabel[eachIndex][0]
            latestGroupLinkEntryType = representLabel[eachIndex][1]
            visitedList.append([latestGroupID, latestGroupLinkEntryType])
            break

          nextSearchGroup = latestGroupID # same group
          # Contrary type point
          nextSearchGroupEntryType = "start" if latestGroupLinkEntryType == "end" else "end"
          visitedList.append([nextSearchGroup, nextSearchGroupEntryType])

          for eachGroup in range(len(previousInfo)):

            # load up current point data
            if nextSearchGroupEntryType == 'start':
              x = previousInfo[nextSearchGroup]['x'][0]
              y = previousInfo[nextSearchGroup]['y'][0]
              data = [x, y]
            else:
              x = previousInfo[nextSearchGroup]['x'][-1]
              y = previousInfo[nextSearchGroup]['y'][-1]
              data = [x, y]

            _, rankIndex = tree.query([data], k=len(dataPoints))
            rankIndex = rankIndex[0] # return 2D list for some reason

            # searching through k closest
            for eachIndex in rankIndex:
              nextGroupID = representLabel[eachIndex][0]
              nextGroupSorE = representLabel[eachIndex][1]
              # print(f"try {latestGroupID},{latestGroupLinkEntryType} to {nextGroupID},{nextGroupSorE}")

              # Avoid reconnection
              if [nextGroupID, nextGroupSorE] in visitedList:
                continue

              """
              (nextGroupID == latestGroupID) avoiding connecting same group
              (referenceStartGroup == nextGroupID) avoiding close loop
              """
              if nextGroupID == latestGroupID or referenceStartGroup == nextGroupID:
                continue
              visitedList.append([nextGroupID, nextGroupSorE])
              nextSearchGroup = nextGroupID
              nextSearchGroupEntryType = "start" if nextGroupSorE == "end" else "end"
              visitedList.append([nextSearchGroup, nextSearchGroupEntryType])
              break

          # load first solution data
          mergeGroupID = visitedList[0][0]
          newSolution = previousInfo[mergeGroupID]['solution']
          newSolutionX = previousInfo[mergeGroupID]['x']
          newSolutionY = previousInfo[mergeGroupID]['y']

          """
          start is front of list merging with another solution
          after means reverse solution so [end,....start] merge another
          will be [end....start...], start is in the middle of solution
          means fully connected degree of 2.
          XY in still represent the city of the solution index (keep)
          """
          mergeEntryType = visitedList[0][1]
          if mergeEntryType == "start":
            newSolution = list(reversed(newSolution))

          for eachMerge in range(1, len(visitedList), 2):

            """
            end is end of list merging with a solution in front,
            is by means merging from end of the list which is in
            reverse (reverse solution),
            """
            # load follow
            mergeGroupID = visitedList[eachMerge][0]
            mergeGroupSolution = previousInfo[mergeGroupID]['solution']
            mergeGroupSolutionX = previousInfo[mergeGroupID]['x']
            mergeGroupSolutionY = previousInfo[mergeGroupID]['y']

            mergeEntryType = visitedList[eachMerge][1]
            if mergeEntryType == "end":
              mergeGroupSolution = list(reversed(mergeGroupSolution))

            """
            when merge XY data is shifted, if is merge back of the list
            XY is shifted in first solution length, to rightly represent
            second solution index XY every solution index in second solution
            + first solution length.
            """
            mergeGroupSolution = [
              eachSolutionIndex + len(newSolution)
              for eachSolutionIndex in mergeGroupSolution
            ]

            # Merge in numpy
            newSolution = np.concatenate([newSolution, mergeGroupSolution])
            newSolutionX = np.concatenate([newSolutionX, mergeGroupSolutionX])
            newSolutionY = np.concatenate([newSolutionY, mergeGroupSolutionY])

          tspGroupXY = {}

          # To one solution descriptions
          tspGroupXY[0] = {
            'solution': newSolution,
            'x': newSolutionX,
            'y': newSolutionY,
            'centroid': None
          }

          # Save
          with open(f'tspGroupXY{step}_{self.dataName}_KDtree.pkl', 'wb') as f:
            pickle.dump(tspGroupXY, f)

          print(f'tspGroupXY{step}_{self.dataName}_KDtree.pkl \"2nd loop\"')

        else:
          if os.path.exists(f'tspGroupXY_{self.dataName}_KDTree.pkl'):
            continue # skip

          print("No")
          """
          First iteration cluster based on
          dataset length.
          """

          for i, eachLine in enumerate(data):
            data[i] = eachLine.replace("\n", "")
            splitData = eachLine.split(" ")
            while True:
              try:
                  splitData.remove("")
              except ValueError:
                  break
            x = float(splitData[1])
            y = float(splitData[2])
            tspData.append([x, y])

          indexSortedAccordingToX = sorted(
            range(len(tspData)),
            key=lambda eachElement: tspData[eachElement][0]
          )

          highestXValue = tspData[indexSortedAccordingToX[0]][0]
          sameXCount = 1

          for i in range(1, len(indexSortedAccordingToX)):
            idx = indexSortedAccordingToX[i]
            if tspData[idx][0] == highestXValue:
              sameXCount += 1
            else:
              break

          topXIndex = indexSortedAccordingToX[:sameXCount]
          highestXsortedByYIndex = sorted(
            topXIndex,
            key=lambda eachIndex:
            tspData[eachIndex][1]
          )
          indexSortedAccordingToX[:sameXCount] = highestXsortedByYIndex

          tspData = [
            tspData[index]
            for index in indexSortedAccordingToX
          ]

          unclusterData = tspData[lenClusterData:]
          tspData = tspData[:lenClusterData]

          clf = KMeansConstrained(
            n_clusters=lenClusterData // self.maxClusterNodeSize,
            size_min=self.maxClusterNodeSize,
            size_max=self.maxClusterNodeSize,
            random_state=0
          )
          clf.fit_predict(np.array(tspData))

          colorList = [
            'b', 'g', 'r', 'c', 'm', 'y', 'tab:blue',
            'tab:orange', 'tab:green', 'tab:red',
            'tab:purple', 'tab:brown', 'tab:pink',
            'tab:gray', 'tab:olive', 'tab:cyan'
          ]
          markerList = [
            'o', 'v', '^', '<', '>', 's',
            'p', '*', 'h', 'H', 'X', 'D',
            'd', 'P', 'x'
          ]

          tspNpArray = np.array(tspData)
          uniqueGroupLabels = list(range(0, lenClusterData // self.maxClusterNodeSize))

          if len(uniqueGroupLabels) > len(markerList) * len(colorList):
            raise Exception("Not enough marker and color combinations for all clusters")

          plt.figure(figsize=(10, 8))

          # dictionary for easier time
          tspGroupXY = {}

          # Plot group
          markerIndex = 0
          colorIndex = 0

          for clusterLabels in uniqueGroupLabels:

            # Each group data
            clusterIncludedData = tspNpArray[clf.labels_ == clusterLabels]

            tspGroupXY[clusterLabels] = {
              'x': clusterIncludedData[:, 0],
              'y': clusterIncludedData[:, 1],
              'centroid': clf.cluster_centers_[clusterLabels]
            }

            marker = markerList[markerIndex]
            color = colorList[colorIndex]

            plt.scatter(
              clusterIncludedData[:, 0], 
              clusterIncludedData[:, 1], 
              marker=marker, 
              color=color, 
              label=f"Cluster {clusterLabels}", 
              s=25
            )

            if markerIndex == len(markerList) - 1:
              markerIndex = 0
              colorIndex += 1
            else:
              markerIndex += 1

          # Last label plot
          if remainUnclusterData != 0:
            labelOfTheGroup = lenClusterData // self.maxClusterNodeSize

            for eachIndex in range(len(unclusterData)):
              x = float(unclusterData[eachIndex][0])
              y = float(unclusterData[eachIndex][1])
              tspData.append([x, y])

            clf = KMeansConstrained(
              n_clusters=1,
              size_min=remainUnclusterData,
              size_max=remainUnclusterData,
              random_state=0
            )
            clf.fit_predict(np.array(tspData[lenOfData - remainUnclusterData:]))

            clusterIncludedData = np.array(tspData[lenOfData - remainUnclusterData:])

            tspGroupXY[labelOfTheGroup] = {
                'x': clusterIncludedData[:, 0],
                'y': clusterIncludedData[:, 1],
                'centroid': clf.cluster_centers_[0]
            }
            uniqueGroupLabels.append(labelOfTheGroup)
            marker = markerList[markerIndex]
            color = colorList[colorIndex]

            plt.scatter(
              clusterIncludedData[:,0], 
              clusterIncludedData[:,1], 
              marker=marker, color=color, 
              label=f"Cluster {labelOfTheGroup}", 
              s=25
            )

          plt.title("Preprocessed TSP graph to subgraph using k-means-constrained")
          plt.xlabel("X")
          plt.ylabel("Y")
          plt.grid(True)
          plt.tight_layout()
          plt.show()

          for eachGroup in tspGroupXY:

            # Fix: fixed starting city issues
            indexSortedAccordingToX = sorted(
              range(len(tspGroupXY[eachGroup]['x'])),
              key=lambda eachElement: tspGroupXY[eachGroup]['x'][eachElement]
            )

            highestXValue = tspGroupXY[eachGroup]['x'][indexSortedAccordingToX[0]]
            sameXCount = 1

            for i in range(1, len(indexSortedAccordingToX)):
              idx = indexSortedAccordingToX[i]
              if tspGroupXY[eachGroup]['x'][idx] == highestXValue:
                sameXCount += 1
              else:
                break

            topXIndex = indexSortedAccordingToX[:sameXCount]
            highestXsortedByYIndex = sorted(
              topXIndex,
              key=lambda eachIndex:
              tspGroupXY[eachGroup]['y'][eachIndex]
            )

            indexSortedAccordingToX[:sameXCount] = highestXsortedByYIndex

            tspGroupXY[eachGroup]['x'] = [
              tspGroupXY[eachGroup]['x'][index]
              for index in indexSortedAccordingToX
            ]
            # Reallocate original pose
            tspGroupXY[eachGroup]['y'] = [
              tspGroupXY[eachGroup]['y'][index]
              for index in indexSortedAccordingToX
            ]

            adjMatrix = np.zeros((
              len(tspGroupXY[eachGroup]['x']),
              len(tspGroupXY[eachGroup]['x'])
            ))

            for i in range(len(tspGroupXY[eachGroup]['x'])):
              for j in range(i + 1, len(tspGroupXY[eachGroup]['x'])):

                distanceIJ = self.calculate_pseudo_distance_based_on_x_y_coordinate (
                  tspGroupXY[eachGroup]['x'][i], tspGroupXY[eachGroup]['x'][j],
                  tspGroupXY[eachGroup]['y'][i], tspGroupXY[eachGroup]['y'][j],
                  weightType=weightType
                )

                adjMatrix[i][j] = distanceIJ
                adjMatrix[j][i] = distanceIJ

            N = len(adjMatrix)

            if N > 2:
              method.statePrepConfig(knownSolution=result[N-2])
              method.problemConfig(
                N=N,
                adjMatrix=adjMatrix,
                problemType='tsp'
              )

              if self.optimizeMethod == "layerwise":
                _, _, _, iterT, tspBestBinary = method.executeLayerWise(self.layerwiseMethod)
              else: # default all layer at once
                _, _, _, iterT, tspBestBinary = method.execute()

              # Decompose solution
              decomposeString = [
                tspBestBinary[i: i + N - 1]
                for i in range(0, len(tspBestBinary), N - 1)
              ]

              # print(decomposeString)
              # To city number
              path = [ 0 for _ in range(N) ]
              for i in range(N - 1):
                forAllOnes = [j for j, value in enumerate(decomposeString[i]) if value == '1']
                path[i + 1] = forAllOnes[0] + 1

              print(f"Group{eachGroup} solution:{path}")

            else:
              path = list(range(N))
              print(f"Group{eachGroup} solution:{path}")
            best = 0
            """
            doesn't matter will recalculate when whole solution
            is QAOA OBJ only has 2-D eud version cost
            """

            tspGroupXY[eachGroup]['solution'] = path
            tspGroupXY[eachGroup]['cost'] = best

          # Save
          with open(f'tspGroupXY_{self.dataName}_KDTree.pkl', 'wb') as f:
            pickle.dump(tspGroupXY, f)
          
          print(f'Saved tspGroupXY_{self.dataName}_KDTree.pkl \"1st loop\"')

          self.draw_route_based_on_solution_data(tspGroupXY)

      # result
      self.draw_route_based_on_solution_data(tspGroupXY, True)
      print(self.solution_total_cost(tspDictionary=tspGroupXY, distanceType=weightType))



    




        




      
  






