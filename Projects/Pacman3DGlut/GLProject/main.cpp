//#define FREEGLUT_STATIC
//#define _LIB
//#define FREEGLUT_LIB_PRAGMAS 0

#include <GL/glut.h>
#include <windows.h>
#include <cstdio>
#include <iostream>
#include <map>
#include <array>
#include <string> 
#include <random>
#define STB_IMAGE_IMPLEMENTATION
#include <stb_image.h>

#define PI 3.141596254

using namespace std;

void initGL();
void display();
void windowChangingPosHandler(int, int);
void keyBoardHandler(unsigned char, int, int);
void idleHandler();
void myViewSubMenu(int id);
void myMainMenu(int id);
void cube(float, float, float);
void solidCube(float, float, float);
void drawNumber(int);
void ghostEventHandler(string);
void reset();

//Shading
GLfloat initialLight0Position[] = { 0, -0.2, -1, 1 };
GLfloat initialLight0Direction[] = { 0, 0, -1 };
GLfloat breathingSpeed = 0.01;  
GLfloat currentColor[3] = { 0.227, 0.353, 1 };
GLfloat neonAmbientAndDiffuse[] = { 0.227, 0.353, 1, 0.5 };
GLfloat neonEmission[] = { .4, .2, 1, 0.3 };
GLfloat neonSpecular[] = { 0, 0, 0, 0.3 };
GLfloat neonShininess[] = { 1 };
GLfloat PacmanAmbientDiffuse[4] = { 1.0, 1.0, 0.0, 1.0 };
GLfloat PacmanSpecular[4] = { 1.0, 1.0, 1.0, 1.0 };
GLfloat PacmanShininess[] = { 50.0 };
GLfloat FoodEmission[] = { 0.0, 0.0, 0.0, 0.0 };
GLfloat PacmanEmission[4] = { 0.976, 0.749, 0.231, 0.3 };
GLfloat PacmanDiffuse[] = { 1, 1, 0, 0.6 };
GLfloat light0Ambient[] = { 1, 1, 1, 1 };
GLfloat light0Position[] = { 0 , -0.2, -1, 1 };
GLfloat light0Direction[] = { 0, 0, 1 };
GLfloat light0Exponent[] = { 0.6 };
GLfloat light0Cutoff[] = { 45 };
GLfloat diffuse[] = { 1, 1, 1, 1 };
GLfloat LEDAmbientAndDiffuse[] = { 0.254, 0.4117, 1, 0.31 };
GLfloat LEDSpecular[] = { 1, 1, 1, 0.51 };
GLfloat LEDShinniness[] = { 31 };
GLfloat LEDEmission[] = { 0, 0, 1, 1 };
GLfloat GhostSpecular[] = { 1,1,1,1 };
GLfloat GhostShinniness[] = { 76 };
GLfloat GhostEmision[][4] = { {1,0,0,0.3},
    {1,0.72156862,1 ,0.3},
    {0,1,1,0.3},
    {0,0,1,1} };
GLfloat GhostDiffuse[][4] = { {0,0,0 ,0.5},
    {0,0,0 ,0.5},
    {0,0,0 ,0.5},
};

//For Texture
const char* imagePath = "C:/Users/zuyua/Downloads/4447734.png";
const char* imagePath3 = "C:/Users/zuyua/Downloads/pacmanTexture5.png";
const char* imagePathForBackground[17] = {
    "frame_00_delay-0.04s.gif",
    "frame_01_delay-0.04s.gif",
    "frame_02_delay-0.04s.gif",
    "frame_03_delay-0.04s.gif",
    "frame_04_delay-0.04s.gif",
    "frame_05_delay-0.04s.gif",
    "frame_06_delay-0.04s.gif",
    "frame_07_delay-0.04s.gif",
    "frame_08_delay-0.04s.gif",
    "frame_09_delay-0.04s.gif",
    "frame_10_delay-0.04s.gif",
    "frame_11_delay-0.04s.gif",
    "frame_12_delay-0.04s.gif",
    "frame_13_delay-0.04s.gif",
    "frame_14_delay-0.04s.gif",
    "frame_15_delay-0.04s.gif",
    "frame_16_delay-0.04s.gif",
};
const char* imagePathForGhostEye1 = "C:/Users/zuyua/Downloads/ghostLeftEyeTexture.png";
const char* imagePathForGhostEye2 = "C:/Users/zuyua/Downloads/ghostRightEyeTexture.png";
const char* imagePathForGhostEye3 = "C:/Users/zuyua/Downloads/ghostScaredLeftEyeTexture.png";
const char* imagePathForGhostEye4 = "C:/Users/zuyua/Downloads/ghostScaredRightEyeTexture.png";
const char* gameOverImagePath = "C:/Users/zuyua/Downloads/game-over-neon.png";
int imageFrame = 4;
int imageWidth, imageHeight, imageChannels;
unsigned char* imageData;
GLuint textureID[25];

//Points
int divisor = 1;
int checkPointsSize = 0, points = 0;

//System
bool gameOver = FALSE;
int viewMode = 3;
double forThridPersonZ = 0;
double forThridPersonX = -0.5;
int devideRadiusBy = 10;
bool pause = FALSE;
double mapRotatedBy = 0.0;
double cameraRotationBy = 0.0;
float cameraHigherOrLowerValue = 0;
float zoomValue = 0;
int level = 1;

//Play enviroment
double outerWallWidth = 0.025;
double outerWallHeight = 0.045;
double widthOfInnerWall = 0.05;
double roadWidth = 0.1;
double eachWallBound[55][4] = {};
//start from 0,0,0
//finish
double myWallPos[][2] = {
    {0.1,0.0875},
    {0.1625,0},
    {0.1, -0.0875},
    {0.3, -0.225},
    {0.225, -0.225},
    {0, -0.3},
    {0, -0.375},
    {0, -0.6125}, //
    {0, -0.7125},
    {0.6875, -0.475},
    {0.5625, -0.2375},
    {0.4375, -0.15},
    {0.5625, -0.0625},
    {0.225, -0.55},
    {0.5, -0.55},
    {0.5, -0.375},
    {-0.1, 0.0875},
    {-0.1625, 0},
    {-0.1, -0.0875},
    {-0.3025, -0.225},
    {-0.225, -0.225},
    {-0.225, -0.55},
    {-0.5025, -0.55},
    {-0.5025, -0.375},
    {-0.565, -0.2375},
    {-0.565, -0.0625},
    {-0.44, -0.15},
    {-0.69, -0.475},
    {0, 0.8125},
    {0, 0.6},
    {0, 0.525}, // // pop pop
    {0.3, 0.6},
    {0.35, 0.675},
    {-0.3, 0.6},
    {-0.35, 0.675},
    {0, 0.3},// // pop
    {0, 0.225},
    {0.225, 0.375},// pop
    {0.3, 0.15},
    {0.45, 0.45}, // pop
    {0.5, 0.375},
    {0.6375, 0.525},
    {0.6875, 0.525},
    {0.5625, 0.2375},
    {0.4375, 0.15},
    {0.5625, 0.0625},
    {-0.235, 0.375},
    {-0.51, 0.375},
    {-0.46, 0.45},
    {-0.64125, 0.525},
    {-0.69125, 0.525},
    {-0.56625, 0.2375},
    {-0.44125, 0.15},
    {-0.30625, 0.15},
    {-0.56625, 0.0625}
};

double wallHWL[][2] = {
    {widthOfInnerWall + roadWidth, outerWallWidth},
    {outerWallWidth, widthOfInnerWall * 2 + roadWidth},
    {widthOfInnerWall + roadWidth, outerWallWidth},
    {widthOfInnerWall, widthOfInnerWall * 3 + roadWidth * 2},
    {widthOfInnerWall * 2 + roadWidth, widthOfInnerWall},
    {widthOfInnerWall, widthOfInnerWall * 2 + roadWidth},
    {widthOfInnerWall * 3 + roadWidth * 2, widthOfInnerWall},
    {widthOfInnerWall, roadWidth * 2 + outerWallWidth},
    {widthOfInnerWall * 7 + roadWidth * 10 + outerWallWidth * 2, outerWallWidth},
    {outerWallWidth, 2 * outerWallWidth + roadWidth * 4 + widthOfInnerWall},
    {roadWidth * 2 + widthOfInnerWall + outerWallWidth, outerWallWidth},
    {outerWallWidth, widthOfInnerWall * 2 + roadWidth},
    {roadWidth * 2 + widthOfInnerWall + outerWallWidth, outerWallWidth},
    {roadWidth + widthOfInnerWall * 2, roadWidth},
    {widthOfInnerWall + roadWidth, roadWidth},
    {widthOfInnerWall + roadWidth, widthOfInnerWall},
    {widthOfInnerWall + roadWidth, outerWallWidth},
    {outerWallWidth, widthOfInnerWall * 2 + roadWidth},
    {widthOfInnerWall + roadWidth, outerWallWidth},
    {widthOfInnerWall, widthOfInnerWall * 3 + roadWidth * 2},
    {widthOfInnerWall * 2 + roadWidth, widthOfInnerWall},
    {roadWidth + widthOfInnerWall * 2, roadWidth},
    {roadWidth + widthOfInnerWall, roadWidth},
    {roadWidth + widthOfInnerWall, widthOfInnerWall},
    {outerWallWidth + roadWidth * 2 + widthOfInnerWall, outerWallWidth},
    {outerWallWidth + roadWidth * 2 + widthOfInnerWall, outerWallWidth},
    {outerWallWidth, widthOfInnerWall * 2 + roadWidth},
    {outerWallWidth, outerWallWidth * 2 + roadWidth * 4 + widthOfInnerWall},
    {widthOfInnerWall * 7 + roadWidth * 10 + outerWallWidth * 2, outerWallWidth},
    {widthOfInnerWall, widthOfInnerWall * 2 + roadWidth},
    {widthOfInnerWall * 3 + roadWidth * 2, widthOfInnerWall},
    {widthOfInnerWall, widthOfInnerWall * 2 + roadWidth},
    {widthOfInnerWall * 3 + roadWidth * 3, widthOfInnerWall},
    {widthOfInnerWall, widthOfInnerWall * 2 + roadWidth},
    {widthOfInnerWall * 3 + roadWidth * 3, widthOfInnerWall},
    {widthOfInnerWall, widthOfInnerWall * 2 + roadWidth},
    {widthOfInnerWall * 3 + roadWidth * 2, widthOfInnerWall},
    {2 * widthOfInnerWall + roadWidth, widthOfInnerWall},
    {widthOfInnerWall, 2 * widthOfInnerWall + roadWidth},
    {widthOfInnerWall, widthOfInnerWall * 2 + roadWidth},
    {widthOfInnerWall + roadWidth, widthOfInnerWall},
    {roadWidth + outerWallWidth, widthOfInnerWall},
    {outerWallWidth, roadWidth * 4 + widthOfInnerWall * 3 + outerWallWidth * 2},
    {roadWidth * 2 + widthOfInnerWall + outerWallWidth, outerWallWidth},
    {outerWallWidth, widthOfInnerWall * 2 + roadWidth},
    {roadWidth * 2 + widthOfInnerWall + outerWallWidth, outerWallWidth},
    {2 * widthOfInnerWall + roadWidth, widthOfInnerWall},
    {roadWidth + widthOfInnerWall, widthOfInnerWall},
    {widthOfInnerWall, widthOfInnerWall * 2 + roadWidth},
    {outerWallWidth + roadWidth, widthOfInnerWall},
    {outerWallWidth, roadWidth * 4 + widthOfInnerWall * 3 + outerWallWidth * 2},
    {outerWallWidth + roadWidth * 2 + widthOfInnerWall, outerWallWidth},
    {outerWallWidth, widthOfInnerWall * 2 + roadWidth},
    {widthOfInnerWall, widthOfInnerWall * 2 + roadWidth},
    {outerWallWidth + roadWidth * 2 + widthOfInnerWall, outerWallWidth}
};

//Pacman
double movingBy = 0.0;
int maxLives = 3;
int currentLives = 3;
double stepsOfCircle = 50;
double pacmanRadius = 0.09;
double degreeOfPackmanMouthOpenOrClose = -45.0;
bool closingMouth = TRUE;
bool immortals = FALSE;
double degreeOfPackmanLives = 0.1;

//ghost
int timer = 50;
double ghostRGB[4][3] = {
    {1,0,0},
    {1,0.72156862,1},
    {0,1,1},
    {1,0.72156862,0.321568}
};
double ghostPos[3][4] = { {} };
int n_legs = 8;

//Food
double bigFoodPos[][3] = {
    {-0.625,0.75},
    { -0.625,0.3 },
    {-0.625,0.6},
    {-0.625,0.45},
    {0.625, 0.3},
    { 0.625,0.45 },
    {0.625,0.6},
    {0.625,0.75},
    { -0.625,-0.45 },
    { 0.625,-0.45 },
};

double foodPos[][3] = {
    {-0.075, -0.55},
    {-0.075, -0.5},
    { -0.625,-0.55},
    { -0.625,-0.5},
    {0.225, -0.35},
    {0.225, -0.4},
    {0.225, 0.5},
    {0.225, 0.55},
    {0.225, 0.6},
    {0.175, 0.6},
    {0.125, 0.6},
    {0.075, 0.6},
    {0.075, 0.65},
    {0.075, 0.7},
    {-0.225, 0.5},
    {-0.225, 0.55},
    {-0.225, 0.6},
    {-0.175, 0.6},
    {-0.125, 0.6},
    {-0.075, 0.6},
    {-0.075, 0.65},
    {-0.075, 0.7},
    { 0.225,-0.05 },
    { 0.225,-0.1 },
    { 0.225,-0.15 },
    { 0.175,-0.15 },
    { 0.125,-0.15 },
    { 0.025,-0.15 },
    { -0.025,-0.15 },
    { -0.075,-0.15 },
    { -0.075,-0.2 },
    { -0.075,-0.25 },
    { -0.125,-0.15 },
    { -0.175,-0.15 },
    { 0.225,0.05 },
    { 0.225,0.1 },
    { 0.225,0.2 },
    { 0.225,0.25 },
    {-0.075, 0.35},
    {-0.075, 0.4},
    { -0.175,0.3 },
    { -0.125,0.3 },
    { -0.075,0.3 },
    { 0.075,0.3 },
    { 0.125,0.3 },
    { 0.175,0.3 },
    { 0.225,0.3 },
    { 0.275,0.3 },
    { 0.325,0.3 },
    { 0.425,0.3 },
    { 0.475,0.3 },
    { 0.525,0.3 },
    { 0.575,0.3 },
    { -0.575,-0.3 },
    { -0.525,-0.3 },
    { -0.475,-0.3 },
    { -0.425,-0.3 },
    { -0.175,-0.3 },
    { -0.125,-0.3 },
    { -0.075,-0.3 },
    { 0.125,-0.3 },
    { 0.175,-0.3 },
    { 0.225,-0.3 },
    { 0.425,-0.3 },
    { 0.475,-0.3 },
    { 0.525,-0.3 },
    { 0.575,-0.3 },
    { -0.575,0 },
    { -0.525,0 },
    { -0.475,0 },
    { -0.425,0 },
    { -0.325,0 },
    { -0.275,0 },
    { 0.225,0 },
    { 0.275,0 },
    { 0.325,0 },
    { 0.425,0 },
    { 0.475,0 },
    { 0.525,0 },
    { 0.575,0 },
    { -0.625,-0.4 },
    { -0.625,-0.35 },
    { -0.625,-0.3 },//
    { -0.625,0 },//
    { -0.175,-0.45 },
    { -0.125,-0.45 },
    { -0.075,-0.45 },
    { -0.025,-0.45 },
    { 0.025,-0.45 },
    { 0.125,-0.45 },
    { 0.175,-0.45 },
    { 0.225,-0.45 },
    { 0.275,-0.45 },
    { 0.325,-0.45 },
    { 0.425,-0.45 },
    { 0.475,-0.45 },
    { 0.525,-0.45 },
    { 0.575,-0.45 },
    { -0.275,-0.45 },
    { -0.325,-0.45 },
    { -0.425,-0.45 },
    { -0.475,-0.45 },
    { -0.525,-0.45 },
    { -0.575,-0.45 },
    //{ -0.625,-0.45 },//
    { -0.225,0.1 },
    { -0.225,0.05 },
    { -0.225,-2.77556e-17 },
    { -0.225,-0.05 },
    { -0.225,-0.1 },
    { -0.225,-0.15 },
    { -0.225,-0.3 },
    { -0.225,-0.35 },
    { -0.225,-0.4 },
    { -0.225,-0.45 },
    { 0.075,-0.55 },
    { 0.075,-0.5 },
    { 0.075,-0.45 },
    { 0.075,-0.3 },
    { 0.075,-0.25 },
    { 0.075,-0.2 },
    { 0.075,-0.15 },
    { 0.075,0.15 },
    { 0.075,0.3 },
    { 0.075,0.35 },
    { 0.075,0.4 },
    { -0.075,-0.6 },
    { 0.075,-0.6 },
    { -0.625,-0.6 },
    {0.225, 0.15},
    {0.175, 0.15},
    {0.125, 0.15},
    {0.075, 0.15},
    {-0.075, 0.15},
    {-0.125, 0.15},
    {-0.175, 0.15},
    {-0.225, 0.15}, //
    {-0.225, 0.2},
    {-0.225, 0.25},
    {-0.225, 0.3}, //
    {-0.275, 0.3},
    {-0.325, 0.3},
    {-0.375, 0.3},
    {-0.425,0.3},
    {-0.475,0.3},
    {-0.525,0.3},
    {-0.575,0.3},
    {-0.625,0.3},
    {-0.625,0.35},
    {-0.625,0.4},
    {-0.575,0.45},
    {-0.525,0.45},
    {-0.525,0.50},
    {-0.525,0.55},
    {-0.525,0.60},
    {-0.575,0.60},
    {-0.625,0.65},
    {-0.625,0.7},
    {-0.575,0.75},
    {-0.525,0.75},
    {-0.475,0.75},
    {-0.425,0.75},
    {-0.375,0.75},
    {-0.325,0.75},
    {-0.275,0.75},
    {-0.225,0.75},
    {-0.175,0.75},
    {-0.125,0.75},
    {-0.075,0.75},
    {-0.025,0.75},
    {0.025,0.75},
    {0.075,0.75},
    {0.125,0.75},
    {0.175,0.75},
    {0.225,0.75},
    {0.275,0.75},
    {0.325,0.75},
    {0.375,0.75},
    {0.425,0.75},
    {0.475,0.75},
    {0.525,0.75},
    {0.575,0.75},
    {0.625,0.7},
    {0.625,0.65},
    {0.575,0.6},
    {0.525,0.6},
    {0.475,0.6},
    { 0.425,0.6 },
    { 0.375,0.6 },
    { 0.375,0.55 },
    { 0.375,0.5 },
    { 0.375,0.45 },
    { 0.375,0.4 },
    { 0.375,0.35 },
    { 0.375,0.3 },
    { 0.375,0.25 },
    { 0.375,0.2 },
    { 0.375,0.15 },
    { 0.375,0.1 },
    { 0.375,0.05 },
    { 0.375,-2.77556e-17 },
    { 0.375,-0.05 },
    { 0.375,-0.1 },
    { 0.375,-0.15 },
    { 0.375,-0.2 },
    { 0.375,-0.25 },
    { 0.375,-0.3 },
    { 0.375,-0.35 },
    { 0.375,-0.4 },
    { 0.375,-0.45 },
    { 0.375,-0.5 },
    { 0.375,-0.55 },
    { 0.375,-0.6 },
    { 0.375,-0.65 },
    { 0.325,-0.65 },
    { 0.275,-0.65 },
    { 0.225,-0.65 },
    { 0.175,-0.65 },
    { 0.125,-0.65 },
    { 0.075,-0.65 },
    { -0.075,-0.65 },
    { -0.125,-0.65 },
    { -0.175,-0.65 },
    { -0.225,-0.65 },
    { -0.275,-0.65 },
    { -0.325,-0.65 },
    { -0.375,-0.65 },
    { -0.425,-0.65 },
    { -0.475,-0.65 },
    { -0.525,-0.65 },
    { -0.575,-0.65 },
    { -0.625,-0.65 },
    { 0.425,-0.65 },
    { 0.475,-0.65 },
    { 0.525,-0.65 },
    { 0.575,-0.65 },
    { 0.625,-0.65 },//
    { 0.625,-0.60 },
    { 0.625,-0.55 },
    { 0.625,-0.5 },
    //{ 0.625,-0.45 },
    { 0.625,-0.4 },
    { 0.625,-0.35 },
    { 0.625,-0.3 },
    { 0.625,1.11022e-16 },
    { 0.625,0.35 },
    { 0.625,0.4 },
    { 0.575,0.45 },
    { 0.525,0.45 },
    { 0.525,0.5 },
    { 0.525,0.55 },
    { 0.375,0.45 },
    { 0.325,0.45 },
    { 0.275,0.45 },
    { 0.225,0.45 },
    { 0.175,0.45 },
    { 0.125,0.45 },
    { 0.075,0.45 },
    { 0.025,0.45 },
    { -0.025,0.45 },
    { -0.075,0.45 },
    { -0.125,0.45 },
    { -0.175,0.45 },
    { -0.225,0.45 },
    { -0.275,0.45 },
    { -0.325,0.45 },
    { -0.375,0.45 },
    { -0.375,0.5 },
    { -0.375,0.55 },
    { -0.375,0.6 },
    { -0.425, 0.6 },
    { -0.475,0.6 },
    { -0.375,0.4 },
    { -0.375,0.35 },
    { -0.375,0.3 },
    { -0.375,0.25 },
    { -0.375,0.2 },
    { -0.375,0.15 },
    { -0.375,0.1 },
    { -0.375,0.05 },
    { -0.375,0 },
    { -0.375,-0.05 },
    { -0.375,-0.1 },
    { -0.375,-0.15 },
    { -0.375,-0.2 },
    { -0.375,-0.25 },
    { -0.375,-0.3 },
    { -0.375,-0.35 },
    { -0.375,-0.4 },
    { -0.375,-0.45 },
    { -0.375,-0.5 },
    { -0.375,-0.55 },
    { -0.375,-0.6 },
    { 0.225, 0 },
};

bool eaten[sizeof(foodPos) / sizeof(foodPos[0]) 
+ sizeof(bigFoodPos) / sizeof(bigFoodPos[0])][1] = { {} };

int numOfArrayRow = sizeof(myWallPos) / sizeof(myWallPos[0]);

//Building Character data
class pacman {
    public:double PosX = 0;
    public:double PosZ = 0;
    public:double offsetX = 0.0;
    public:double offsetZ = 0.0;
    public:double facingRotationBy;
    public:bool dead = FALSE;
    public:bool didHit = FALSE;
    public:double boundOftheCharacter[1][4];

    void characterBound() {
        //X-min
        boundOftheCharacter[0][0] = PosX - 0.09 / 2.0;
        //X-max
        boundOftheCharacter[0][1] = PosX + 0.09 / 2.0;
        //Z-max
        boundOftheCharacter[0][2] = PosZ + 0.09 / 2.0;
        //Z-min
        boundOftheCharacter[0][3] = PosZ - 0.09 / 2.0;
    }

    void throughTunnel() {
        if (boundOftheCharacter[0][1] >= 0.7)
            PosX = -0.6;
        if (boundOftheCharacter[0][0] <= -0.7)
            PosX = 0.6;
    }

    //TODO
    void checkDidHit() {
        characterBound();
        for (int i = 0; i < sizeof(myWallPos) / sizeof(myWallPos[0]); i++) {
            if (eachWallBound[i][1] - eachWallBound[i][0] < 0.1) { //x slimer than character
                if (((boundOftheCharacter[0][1] > eachWallBound[i][1]
                    && boundOftheCharacter[0][0] < eachWallBound[i][1])
                    ||
                    (boundOftheCharacter[0][1] > eachWallBound[i][0]
                        && boundOftheCharacter[0][0] < eachWallBound[i][0])
                    ) && ((eachWallBound[i][2] > boundOftheCharacter[0][2]
                        && eachWallBound[i][3] < boundOftheCharacter[0][2])
                        ||
                        (eachWallBound[i][2] > boundOftheCharacter[0][3]
                            && eachWallBound[i][3] < boundOftheCharacter[0][3]))) {
                    didHit = true;
                    break;
                    //cout << "Hit" << endl;
                }
            }
            else if (eachWallBound[i][2] - eachWallBound[i][3] < 0.1) { //z slimer than character
                if (((boundOftheCharacter[0][2] > eachWallBound[i][2]
                    && boundOftheCharacter[0][3] < eachWallBound[i][2])
                    ||
                    (boundOftheCharacter[0][2] > eachWallBound[i][3]
                        && boundOftheCharacter[0][3] < eachWallBound[i][3])
                    ) && ((eachWallBound[i][1] > boundOftheCharacter[0][1]
                        && eachWallBound[i][0] < boundOftheCharacter[0][1])
                        ||
                        (eachWallBound[i][1] > boundOftheCharacter[0][0]
                            && eachWallBound[i][0] < boundOftheCharacter[0][0]))) {
                    didHit = true;
                    break;
                    //cout << "Hit" << endl;
                }
            }
            else { // both equal
                if ((eachWallBound[i][1] > boundOftheCharacter[0][0]  // x-min check ok
                    && boundOftheCharacter[0][0] > eachWallBound[i][0] &&
                    eachWallBound[i][2] > boundOftheCharacter[0][3] && // z-min check ok
                    boundOftheCharacter[0][3] > eachWallBound[i][3])
                    ||
                    (eachWallBound[i][1] > boundOftheCharacter[0][1]  // x-max check ok
                        && boundOftheCharacter[0][1] > eachWallBound[i][0] &&
                        eachWallBound[i][2] > boundOftheCharacter[0][2] && // z-max check ok
                        boundOftheCharacter[0][2] > eachWallBound[i][3])
                    ||
                    (eachWallBound[i][1] > boundOftheCharacter[0][1]
                        && boundOftheCharacter[0][1] > eachWallBound[i][0] &&
                        eachWallBound[i][2] > boundOftheCharacter[0][3] &&
                        boundOftheCharacter[0][3] > eachWallBound[i][3])
                    ||
                    (eachWallBound[i][1] > boundOftheCharacter[0][0]
                        && boundOftheCharacter[0][0] > eachWallBound[i][0] &&
                        eachWallBound[i][2] > boundOftheCharacter[0][2] &&
                        boundOftheCharacter[0][2] > eachWallBound[i][3])
                    ) {
                    didHit = true;
                    break;
                    //cout << "Hit" << endl;
                }
            }
        }
    }

    void checkDidEat() {
        for (int i = 0; i < sizeof(foodPos) / sizeof(foodPos[0]); i++) {
            if ((boundOftheCharacter[0][1] >= foodPos[i][0] && boundOftheCharacter[0][0] <= foodPos[i][0])
                && (boundOftheCharacter[0][2] >= foodPos[i][1] && boundOftheCharacter[0][3] <= foodPos[i][1])) {
                if (!eaten[i][0]) {
                    eaten[i][0] = TRUE;
                    points += 2;
                    cout << "eaten:" << foodPos[i][0] << " " << foodPos[i][1] << endl;
                }
            }
        }

        for (int i = 0; i < sizeof(bigFoodPos) / sizeof(bigFoodPos[0]); i++) {
            if ((boundOftheCharacter[0][1] >= bigFoodPos[i][0] && boundOftheCharacter[0][0] <= bigFoodPos[i][0])
                && (boundOftheCharacter[0][2] >= bigFoodPos[i][1] && boundOftheCharacter[0][3] <= bigFoodPos[i][1])) {
                if (!eaten[sizeof(foodPos) / sizeof(foodPos[0]) + i][0]) {
                    eaten[sizeof(foodPos) / sizeof(foodPos[0]) + i][0] = TRUE;
                    points += 4;
                    ghostEventHandler("Pacman Eat Big Food");
                }
            }
        }
    }

};

struct ghost
{
    double PosX = 0;
    double PosZ = 0;
    bool didHit = FALSE;
    bool dead = FALSE;
    bool scared = FALSE;
    bool isRelease = FALSE;
    int scaredTimer;
    int movementDirectionType;
    double boundOftheCharacter[1][4];

    void characterBound(){
        //X-min
        boundOftheCharacter[0][0] = PosX - 0.09 / 2.0;
        //X-max
        boundOftheCharacter[0][1] = PosX + 0.09 / 2.0;
        //Z-max
        boundOftheCharacter[0][2] = PosZ + 0.09 / 2.0;
        //Z-min
        boundOftheCharacter[0][3] = PosZ - 0.09 / 2.0;
    }

    void throughTunnel() {
        if (boundOftheCharacter[0][1] >= 0.7)
            PosX = -0.6;
        if (boundOftheCharacter[0][0] <= -0.7)
            PosX = 0.6;
    }

    void checkDidHitPacman(pacman& Pacman) {
        characterBound();
        if (boundOftheCharacter[0][1] > Pacman.boundOftheCharacter[0][0] &&
            boundOftheCharacter[0][0] < Pacman.boundOftheCharacter[0][1] &&
            boundOftheCharacter[0][2] > Pacman.boundOftheCharacter[0][3] &&
            boundOftheCharacter[0][3] < Pacman.boundOftheCharacter[0][2]) {
            if (scared) {
                dead = TRUE;
                points += 10;
            }
            else
                if(!immortals)
                    Pacman.dead = TRUE;
        }
    }

    //TODO
    void checkDidHit() {
        characterBound();
        for (int i = 0; i < sizeof(myWallPos) / sizeof(myWallPos[0]); i++) {
            if (eachWallBound[i][1] - eachWallBound[i][0] < 0.1) { //x slimer than character
                if (((boundOftheCharacter[0][1] > eachWallBound[i][1]
                    && boundOftheCharacter[0][0] < eachWallBound[i][1])
                    || 
                    (boundOftheCharacter[0][1] > eachWallBound[i][0]
                    && boundOftheCharacter[0][0] < eachWallBound[i][0])
                    ) && ((eachWallBound[i][2] > boundOftheCharacter[0][2]
                    && eachWallBound[i][3] < boundOftheCharacter[0][2])
                    || 
                    (eachWallBound[i][2] > boundOftheCharacter[0][3]
                    && eachWallBound[i][3] < boundOftheCharacter[0][3]))) {
                    didHit = true;
                    break;
                    //cout << "Hit" << endl;
                }
            }
            else if (eachWallBound[i][2] - eachWallBound[i][3] < 0.1) { //z slimer than character
                if (((boundOftheCharacter[0][2] > eachWallBound[i][2]
                    && boundOftheCharacter[0][3] < eachWallBound[i][2])
                    || 
                    (boundOftheCharacter[0][2] > eachWallBound[i][3]
                    && boundOftheCharacter[0][3] < eachWallBound[i][3])
                    ) && ((eachWallBound[i][1] > boundOftheCharacter[0][1]
                    && eachWallBound[i][0] < boundOftheCharacter[0][1])
                    || 
                    (eachWallBound[i][1] > boundOftheCharacter[0][0]
                    && eachWallBound[i][0] < boundOftheCharacter[0][0]))) {
                    didHit = true;
                    break;
                    //cout << "Hit" << endl;
                }
            }
            else { // both equal
                if ((eachWallBound[i][1] > boundOftheCharacter[0][0]  // x-min check ok
                    && boundOftheCharacter[0][0] > eachWallBound[i][0] &&
                    eachWallBound[i][2] > boundOftheCharacter[0][3] && // z-min check ok
                    boundOftheCharacter[0][3] > eachWallBound[i][3])
                    ||
                    (eachWallBound[i][1] > boundOftheCharacter[0][1]  // x-max check ok
                    && boundOftheCharacter[0][1] > eachWallBound[i][0] &&
                    eachWallBound[i][2] > boundOftheCharacter[0][2] && // z-max check ok
                    boundOftheCharacter[0][2] > eachWallBound[i][3])
                    ||
                    (eachWallBound[i][1] > boundOftheCharacter[0][1] 
                    && boundOftheCharacter[0][1] > eachWallBound[i][0] &&
                    eachWallBound[i][2] > boundOftheCharacter[0][3] && 
                        boundOftheCharacter[0][3] > eachWallBound[i][3])
                    ||
                    (eachWallBound[i][1] > boundOftheCharacter[0][0] 
                    && boundOftheCharacter[0][0] > eachWallBound[i][0] &&
                    eachWallBound[i][2] > boundOftheCharacter[0][2] && 
                    boundOftheCharacter[0][2] > eachWallBound[i][3])
                    ) {
                    didHit = true;
                    break;
                    //cout << "Hit" << endl;
               }
            }
        }
    }
};

pacman Pacman;
struct ghost Blinky;
struct ghost Pinky;
struct ghost Inky;

//map<int64_t, array<double, 2>> foodPos = {
//        { 1, {0.225, 0.15}},
//        { 2, {0.125, 0.15}},
//        { 3, {0.025, 0.15}}
//};

void ghostEventHandler(string eventName) {
    if (eventName == "Pacman Eat Big Food") {
        Blinky.scared = TRUE;
        Pinky.scared = TRUE;
        Inky.scared = TRUE;

        for (int i = 0; i < 3; i++) {
            ghostRGB[i][0] = 0;
            ghostRGB[i][1] = 0;
            ghostRGB[i][2] = 1;
        }

        Blinky.scaredTimer = 500;
        Pinky.scaredTimer = 500;
        Inky.scaredTimer = 500;
    }
}

void specialKeyHandler(int key, int x, int y) {
    switch (key) {
    case GLUT_KEY_UP:
        zoomValue -= 0.1;
        break;
    case GLUT_KEY_DOWN:
        zoomValue += 0.1;
        break;
    }
}

//Finish: Control Pacman
void keyBoardHandler(unsigned char key, int x, int y) {
    if (key == ' ') {
        myMainMenu(6);
        glutIdleFunc(idleHandler);
        gameOver = FALSE;
    }
    //Rotate Map Clock Wise
    if (key == 'q') {
        mapRotatedBy -= 1;
        cameraRotationBy -= 0.1;
    }
    //Rotate Map Counter Clock Wise
    if (key == 'e') {
        mapRotatedBy += 1;
        cameraRotationBy += 0.1;
    }
    if (key == 'w') {
        Pacman.facingRotationBy = 90;
        movingBy = -0.01;
        forThridPersonZ = 0.5;
        forThridPersonX = 0;
    }
    if (key == 's') {
        Pacman.facingRotationBy = -90;
        movingBy = 0.01;
        forThridPersonZ = -0.5;
        forThridPersonX = 0;
    }
    if (key == 'd') {
        Pacman.facingRotationBy = 0;
        movingBy = 0.01;
        forThridPersonZ = 0;
        forThridPersonX = -0.5;
    }
    if (key == 'a') {
        Pacman.facingRotationBy = 180;
        movingBy = -0.01;
        forThridPersonZ = 0;
        forThridPersonX = 0.5;
    }
    if (key == 'k') { // lower camera
        cameraHigherOrLowerValue -= 0.1;
    }
    if (key == 'i') { // lower camera
        cameraHigherOrLowerValue += 0.1;
    }
    switch (key)
    {
    case 27: // Escape key
        if (pause) {
            pause = FALSE;
            glutIdleFunc(idleHandler);
        }
        else {
            pause = TRUE;
            glutIdleFunc(NULL);
        }
    default:
        break;
    }
    glutPostRedisplay();
}

//Finish
void loadImage(const char* imagePath) {
    stbi_set_flip_vertically_on_load(true);
    imageData = stbi_load(imagePath, &imageWidth, &imageHeight, &imageChannels, 0);
    if (!imageData) {
        std::cout << "Failed to load image: " << imagePath << std::endl;
        exit(1);
    }
}

//TODO Menu changing View zoom out or in
void initGL() {
    int viewSubMenu = glutCreateMenu(myViewSubMenu);
    glutAddMenuEntry("Third Person", 2);
    glutAddMenuEntry("Far away following Pacman", 3);
    glutAddMenuEntry("Bird Eye", 4);
    glutAddMenuEntry("Full 45 degree 3D View", 5);

    glutCreateMenu(myMainMenu);
    glutAddMenuEntry("Quit", 1);
    glutAddMenuEntry("Reset", 6);
    glutAddMenuEntry("Level up!", 7);
    glutAddMenuEntry("Immortal on/off", 8);
    glutAddSubMenu("Changing view", viewSubMenu);

    glutAttachMenu(GLUT_RIGHT_BUTTON);

    glGenTextures(25, textureID);
    loadImage(imagePath);
    //glGenTextures(1, &textureID[0]);
    glBindTexture(GL_TEXTURE_2D, textureID[0]);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imageWidth, imageHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);

    //loadImage(imagePath2);
    //glGenTextures(1, &textureID[1]);
    //glBindTexture(GL_TEXTURE_2D, textureID[1]);
    /*glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imageWidth, imageHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);*/

    loadImage(imagePath3);
    //glGenTextures(1, &textureID[2]);
    glBindTexture(GL_TEXTURE_2D, textureID[2]);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imageWidth, imageHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);

    for (int i = 0; i <= 16; i++) {
        loadImage(imagePathForBackground[i]);
        //glGenTextures(1, &textureID[i + 3]);
        glBindTexture(GL_TEXTURE_2D, textureID[i + 3]);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imageWidth, imageHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);
    }


    loadImage(imagePathForGhostEye1);

    //glGenTextures(1, &textureID[20]);
    glBindTexture(GL_TEXTURE_2D, textureID[20]);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imageWidth, imageHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);

    loadImage(imagePathForGhostEye2);

    //glGenTextures(1, &textureID[21]);
    glBindTexture(GL_TEXTURE_2D, textureID[21]);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imageWidth, imageHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);

    loadImage(imagePathForGhostEye3);

    //glGenTextures(1, &textureID[22]);
    glBindTexture(GL_TEXTURE_2D, textureID[22]);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imageWidth, imageHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);

    loadImage(imagePathForGhostEye4);

    //glGenTextures(1, &textureID[23]);
    glBindTexture(GL_TEXTURE_2D, textureID[23]);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imageWidth, imageHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);

    loadImage(gameOverImagePath);

    //glGenTextures(1, &textureID[23]);
    glBindTexture(GL_TEXTURE_2D, textureID[24]);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, imageWidth, imageHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);

    glShadeModel(GL_SMOOTH);
    glLightModeli(GL_LIGHT_MODEL_TWO_SIDE, GL_TRUE);
    glLightModeli(GL_LIGHT_MODEL_LOCAL_VIEWER, GL_TRUE);


    glClearColor(0, 0, 0, 0);
    glColor3f(1.0, 1.0, 1.0);
    glViewport(0, 0, glutGet(GLUT_WINDOW_WIDTH), glutGet(GLUT_WINDOW_HEIGHT));
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluPerspective(45, 1, 0.5, 100);
    /*glEnable(GL_LIGHTING);
    glEnable(GL_LIGHT0);*/
    /*glLightfv(GL_LIGHT0, GL_AMBIENT, light0Ambient);
    glLightfv(GL_LIGHT0, GL_DIFFUSE, diffuse);
    glLightfv(GL_LIGHT0, GL_POSITION, light0Position);
    glLightfv(GL_LIGHT0, GL_SPOT_DIRECTION, light0Direction);
    glLightfv(GL_LIGHT0, GL_SPOT_CUTOFF, light0Cutoff);
    glLightfv(GL_LIGHT0, GL_SPOT_EXPONENT, light0Exponent);*/
    cout << light0Position[0] << "¡@" << light0Position[1] << "¡@" << light0Position[2] << "¡@" << endl;
}

//Finish
void myViewSubMenu(int id) {
    if (id == 2) {
        viewMode = 1;
    }
    else if (id == 3) {
        viewMode = 2;
    }
    else if (id == 4) {
        viewMode = 3;
    }
    else if (id == 5) {
        viewMode = 4;
    }

    //cout << light0Position << endl;
    glutPostRedisplay();
}

//Todo:Reset
void myMainMenu(int id) {
    if (id == 1) {
        exit(0);
    }
    else if (id == 6) {
        for (int i = 0; i < sizeof(foodPos) / sizeof(foodPos[0])
            + sizeof(bigFoodPos) / sizeof(bigFoodPos[0]); i++) {
            eaten[i][0] = FALSE;
        }
        currentLives = maxLives;
        points = 0;
        level = 1;
        reset();
    }
    else if (id == 7) {
        level++;
    }
    else if (id == 8) {
        immortals = !immortals;
    }
}

//Finish
void drawNumber(int number) {
    glDisable(GL_LIGHTING);
    glDisable(GL_LIGHT0);
    switch (number)
    {
    case 0:
        glTranslatef(0, -0.06, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glPushMatrix();
        glPushMatrix();
        glTranslatef(0.0375, -0.04, 0);
        solidCube(widthOfInnerWall / 2, 0.1, 0);
        glPopMatrix();
        glTranslatef(-0.0375, -0.04, 0);
        solidCube(widthOfInnerWall / 2, 0.1, 0);
        glPopMatrix();
        glTranslatef(0, -0.08, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        break;
    case 1:
        glTranslatef(0.05, -0.1, 0);
        solidCube(widthOfInnerWall / 2, 0.1, 0);
        break;
    case 2:
        glPushMatrix();
        glPushMatrix();
        glTranslatef(0, -0.06, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0.04, -0.02, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(-0.04, -0.12, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(0, -0.1, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0, -0.04, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        break;
    case 3:
        glPushMatrix();
        glPushMatrix();
        glTranslatef(0, -0.06, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0.04, -0.02, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(0.04, -0.12, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(0, -0.1, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0, -0.04, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        break;
    case 4:
        glPushMatrix();
        glTranslatef(-0.04, -0.08, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glTranslatef(0.04, -0.02, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glPopMatrix();
        glTranslatef(0.04, -0.1, 0);
        solidCube(widthOfInnerWall / 2, 0.1, 0);
        break;
    case 5:
        glRotatef(180, 0, 1, 0);
        glPushMatrix();
        glPushMatrix();
        glTranslatef(0, -0.06, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0.04, -0.02, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(-0.04, -0.12, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(0, -0.1, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0, -0.04, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        break;
    case 6:
        glTranslatef(0, -0.06, 0);
        glPushMatrix();
        glPushMatrix();
        glTranslatef(0.0375, -0.06, 0);
        solidCube(widthOfInnerWall / 2, 0.05, 0);
        glPopMatrix();
        glTranslatef(-0.0375, -0.04, 0);
        solidCube(widthOfInnerWall / 2, 0.1, 0);
        glPopMatrix();
        glTranslatef(0, -0.08, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0, 0.04, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        break;
    case 7:
        glTranslatef(0, -0.06, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glPushMatrix();
        glPushMatrix();
        glTranslatef(0.0375, -0.04, 0);
        solidCube(widthOfInnerWall / 2, 0.1, 0);
        glPopMatrix();
        glPopMatrix();
        break;
    case 8:
        glPushMatrix();
        glPushMatrix();
        glTranslatef(0, -0.06, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0.04, -0.02, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(-0.04, -0.12, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(0, -0.1, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0, -0.04, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);

        glRotatef(180, 0, 1, 0);
        glTranslatef(0, 0.14, 0);
        glPushMatrix();
        glPushMatrix();
        glTranslatef(0, -0.06, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0.04, -0.02, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(-0.04, -0.12, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glPopMatrix();
        glTranslatef(0, -0.1, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glTranslatef(0, -0.04, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        break;
    case 9:
        glPushMatrix();
        glTranslatef(0, -0.06, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glPopMatrix();
        glPushMatrix();
        glTranslatef(-0.04, -0.08, 0);
        solidCube(widthOfInnerWall / 2, 0.065, 0);
        glTranslatef(0.04, -0.02, 0);
        solidCube(widthOfInnerWall * 2, 0.025, 0);
        glPopMatrix();
        glTranslatef(0.04, -0.1, 0);
        solidCube(widthOfInnerWall / 2, 0.1, 0);
        break;
    }
    glEnable(GL_LIGHTING);
    glEnable(GL_LIGHT0);
}

//Finish
void wireCube(float x, float y, float z) {
    glPushMatrix();
    glScalef(x, y, z);
    glutWireCube(1.0);
    glPopMatrix();
}

//Finish
void cube(float x, float y, float z) {
    glPushMatrix();
    glScalef(x, y, z);
    glScalef(0.5, 0.5, 0.5);
    //glColor3f(0.227, 0.353, 1);
    glColor3f(0, 0, 0);
    glLineWidth(5);
    glDisable(GL_LIGHTING);

    // Render the neon strips with the glowing effect

    glMaterialfv(GL_FRONT, GL_AMBIENT_AND_DIFFUSE, neonAmbientAndDiffuse);
    glMaterialfv(GL_FRONT, GL_EMISSION, neonEmission);
    glMaterialfv(GL_FRONT, GL_SPECULAR, neonSpecular);
    glMaterialfv(GL_FRONT, GL_SHININESS, neonShininess);
    glBegin(GL_LINE_LOOP);
    glVertex3f(-1, -1, 1);
    glVertex3f(1, -1, 1);
    glVertex3f(1, -1, -1);
    glVertex3f(-1, -1, -1);
    glEnd();
    glColor3f(0, 0, 0);
    glBegin(GL_POLYGON);
    glVertex3f(-1, 1, 1);
    glVertex3f(-1, -1, 1);
    glVertex3f(-1, -1, -1);
    glVertex3f(-1, 1, -1);
    glEnd();
    glBegin(GL_POLYGON);
    glVertex3f(-1, 1, -1);
    glVertex3f(1, 1, -1);
    glVertex3f(1, -1, -1);
    glVertex3f(-1, -1, -1);
    glEnd();
    glBegin(GL_POLYGON);
    glVertex3f(1, 1, 1);
    glVertex3f(1, 1, -1);
    glVertex3f(1, -1, -1);
    glVertex3f(1, -1, 1);
    glEnd();
    glBegin(GL_POLYGON);
    glVertex3f(1, 1, 1);
    glVertex3f(1, 1, -1);
    glVertex3f(1, -1, -1);
    glVertex3f(1, -1, 1);
    glEnd();
    glBegin(GL_POLYGON);
    glVertex3f(1, 1, 1);
    glVertex3f(1, -1, 1);
    glVertex3f(-1, -1, 1);
    glVertex3f(-1, 1, 1);
    glEnd();
    glBegin(GL_POLYGON);
    glVertex3f(1, 1, 1);
    glVertex3f(-1, 1, 1);
    glVertex3f(-1, 1, -1);
    glVertex3f(1, 1, -1);
    glEnd();
    glColor3f(0.227, 0.353, 1);
    glLineWidth(5);
    glEnable(GL_LIGHTING);
    glEnable(GL_LIGHT0);
    glBegin(GL_LINE_LOOP);
    glVertex3f(-1, 1, -1);
    glVertex3f(1, 1, -1);
    glVertex3f(1, 1, 1);
    glVertex3f(-1, 1, 1);
    glEnd();
    glPopMatrix();
}

//Finish
void solidCube(float x, float y, float z) {
    glPushMatrix();
    glScalef(x, y, z);
    glutSolidCube(1.0);
    glPopMatrix();
}

//Finish
void sphere(double radius,double phiStart) {
    // formula shpere
    // x = r * sin(phi) * cos(theta)
    // y = r * sin(phi) * sin(theta)
    // z = r * cos(phi)
    
    // ideas using half circle and rotate it by z
    glEnable(GL_TEXTURE_2D);
    glBindTexture(GL_TEXTURE_2D, 3);
    

    glPushMatrix();
    for (double i = phiStart; i < 180 - (phiStart); i += 1) {
        
        glPushMatrix();
        glRotatef(i, 0, 0, 1);
        glRotatef(90, 0, 1, 0);
        glBegin(GL_POLYGON);
        for (int j = 0; j <= stepsOfCircle; j++) {
            float theta = j * (PI / stepsOfCircle);
            float x = radius * cos(theta);
            float y = radius * sin(theta);

            float u = (i - phiStart) / (180 - 2 * phiStart);
            float v = (stepsOfCircle - j) / stepsOfCircle;

            glTexCoord2f(u, v);
            glVertex2f(x, y);
        }
        glEnd();
        glPopMatrix();
    }
    glPopMatrix();

    glDisable(GL_TEXTURE_2D);
}

//Finish
void windowChangingPosHandler(int width, int height) {
    glViewport(0, 0, glutGet(GLUT_WINDOW_WIDTH), glutGet(GLUT_WINDOW_HEIGHT));
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluPerspective(45, 1, -0.5, 100);
    glutPostRedisplay();
}

//TODO: MovePacmanBy, MoveGhostBy, AI Algorithm for ghost
void reset() {
    Pacman.PosX = 0;
    Pacman.PosZ = (widthOfInnerWall * 2 + roadWidth) / 2.0 + roadWidth / 2.0;
    Pacman.facingRotationBy = 0;
    degreeOfPackmanMouthOpenOrClose = -45.0;
    movingBy = 0;
    Blinky.PosX = 0.085;
    Blinky.PosZ = 0;
    Pinky.PosX = Blinky.PosX - 0.09;
    Pinky.PosZ = 0;
    Inky.PosX = Pinky.PosX - 0.09;
    Inky.PosZ = 0;
    Blinky.isRelease = FALSE;
    Inky.isRelease = FALSE;
    Pinky.isRelease = FALSE;
    Pacman.dead = FALSE;
    Blinky.scaredTimer = 1;
    Inky.scaredTimer = 1;
    Pinky.scaredTimer = 1;
    timer = 50 * level;
}

void idleHandler() {
    timer--;
    Blinky.scaredTimer--;
    Pinky.scaredTimer--;
    Inky.scaredTimer--;

    for (int i = 0; i < 3; i++) {
        currentColor[i] += breathingSpeed;
        if (currentColor[i] > 1.0) {
            currentColor[i] = 1.0;
            breathingSpeed = -breathingSpeed;
        }
        else if (currentColor[i] < 0.0) {
            currentColor[i] = 0.0;
            breathingSpeed = -breathingSpeed;
        }
    }

    neonAmbientAndDiffuse[0] = currentColor[0];
    neonAmbientAndDiffuse[1] = currentColor[1];
    neonAmbientAndDiffuse[2] = currentColor[2];

    if (!Pacman.dead && !Blinky.dead && !Inky.dead && !Pinky.dead) {
        degreeOfPackmanLives += 5.0;

        if (Blinky.scaredTimer == 0) {
            ghostRGB[0][0] = 1;
            ghostRGB[0][1] = 0;
            ghostRGB[0][2] = 0;
            Blinky.scared = FALSE;
        }
        if (Pinky.scaredTimer == 0) {
            ghostRGB[1][0] = 1;
            ghostRGB[1][1] = 0.72156862;
            ghostRGB[1][2] = 1;
            Pinky.scared = FALSE;
        }
        if (Inky.scaredTimer == 0) {
            ghostRGB[2][0] = 0;
            ghostRGB[2][1] = 1;
            ghostRGB[2][2] = 1;
            Inky.scared = FALSE;
        }

        if (timer == 0) {
            if (!Blinky.isRelease) {
                Blinky.PosX = 0;
                Blinky.PosZ = (widthOfInnerWall * 2 + roadWidth) / 2.0 + roadWidth / 2.0;
                Blinky.isRelease = TRUE;
                timer = 50 * level;
            }
            else if (!Inky.isRelease) {
                Inky.PosX = 0;
                Inky.PosZ = (widthOfInnerWall * 2 + roadWidth) / 2.0 + roadWidth / 2.0;
                Inky.isRelease = TRUE;
                timer = 50 * level;
            }
            else if (!Pinky.isRelease) {
                Pinky.PosX = 0;
                Pinky.PosZ = (widthOfInnerWall * 2 + roadWidth) / 2.0 + roadWidth / 2.0;
                Pinky.isRelease = TRUE;
                timer = 50 * level;
            }
        }

        srand(time(NULL));
        switch (Blinky.movementDirectionType) {
        case 0:
            Blinky.PosX -= 0.01;
            Blinky.checkDidHit();
            Blinky.throughTunnel();
            if (Blinky.didHit) {
                Blinky.PosX += 0.01;
                Blinky.movementDirectionType = rand() % 4;
            }
            break;
        case 1:
            Blinky.PosX += 0.01;
            Blinky.checkDidHit();
            Blinky.throughTunnel();
            if (Blinky.didHit) {
                Blinky.PosX -= 0.01;
                Blinky.movementDirectionType = rand() % 4;
            }
            break;
        case 2:
            Blinky.PosZ -= 0.01;
            Blinky.checkDidHit();
            Blinky.throughTunnel();
            if (Blinky.didHit) {
                Blinky.PosZ += 0.01;
                Blinky.movementDirectionType = rand() % 4;
            }
            break;
        case 3:
            Blinky.PosZ += 0.01;
            Blinky.checkDidHit();
            Blinky.throughTunnel();
            if (Blinky.didHit) {
                Blinky.PosZ -= 0.01;
                Blinky.movementDirectionType = rand() % 4;
            }
            break;
        }

        switch (Inky.movementDirectionType) {
        case 0:
            Inky.PosX -= 0.01;
            Inky.checkDidHit();
            Inky.throughTunnel();
            if (Inky.didHit) {
                Inky.PosX += 0.01;
                Inky.movementDirectionType = rand() % 4;
            }
            break;
        case 1:
            Inky.PosX += 0.01;
            Inky.checkDidHit();
            Inky.throughTunnel();
            if (Inky.didHit) {
                Inky.PosX -= 0.01;
                Inky.movementDirectionType = rand() % 4;
            }
            break;
        case 2:
            Inky.PosZ -= 0.01;
            Inky.checkDidHit();
            Inky.throughTunnel();
            if (Inky.didHit) {
                Inky.PosZ += 0.01;
                Inky.movementDirectionType = rand() % 4;
            }
            break;
        case 3:
            Inky.PosZ += 0.01;
            Inky.checkDidHit();
            Inky.throughTunnel();
            if (Inky.didHit) {
                Inky.PosZ -= 0.01;
                Inky.movementDirectionType = rand() % 4;
            }
            break;
        }
        switch (Pinky.movementDirectionType) {
        case 0:
            Pinky.PosX -= 0.01;
            Pinky.checkDidHit();
            Pinky.throughTunnel();
            if (Pinky.didHit) {
                Pinky.PosX += 0.01;
                Pinky.movementDirectionType = rand() % 4;
            }
            break;
        case 1:
            Pinky.PosX += 0.01;
            Pinky.checkDidHit();
            Pinky.throughTunnel();
            if (Pinky.didHit) {
                Pinky.PosX -= 0.01;
                Pinky.movementDirectionType = rand() % 4;
            }
            break;
        case 2:
            Pinky.PosZ -= 0.01;
            Pinky.checkDidHit();
            Pinky.throughTunnel();
            if (Pinky.didHit) {
                Pinky.PosZ += 0.01;
                Pinky.movementDirectionType = rand() % 4;
            }
            break;
        case 3:
            Pinky.PosZ += 0.01;
            Pinky.checkDidHit();
            Pinky.throughTunnel();
            if (Pinky.didHit) {
                Pinky.PosZ -= 0.01;
                Pinky.movementDirectionType = rand() % 4;
            }
            break;
        }

        if (imageFrame == 18) {
            imageFrame = 4;
        }
        else {
            imageFrame++;
        }

        if (movingBy == -0.01) {
            if (Pacman.facingRotationBy == 90) {
                Pacman.offsetZ += movingBy;
                Pacman.PosZ += movingBy;
                Pacman.checkDidHit();
                Pacman.checkDidEat();
                Pacman.throughTunnel();
                if (Pacman.didHit) {
                    Pacman.offsetZ -= movingBy;
                    Pacman.PosZ -= movingBy;
                }
            }
            else {
                Pacman.offsetX += movingBy;
                Pacman.PosX += movingBy;
                Pacman.checkDidHit();
                Pacman.checkDidEat();
                Pacman.throughTunnel();
                if (Pacman.didHit) {
                    Pacman.offsetX -= movingBy;
                    Pacman.PosX -= movingBy;
                }
            }
        }
        else {
            if (Pacman.facingRotationBy == 0) {
                Pacman.offsetX += movingBy;
                Pacman.PosX += movingBy;
                Pacman.checkDidHit();
                Pacman.checkDidEat();
                Pacman.throughTunnel();
                if (Pacman.didHit) {
                    Pacman.offsetX -= movingBy;
                    Pacman.PosX -= movingBy;
                }
            }
            else {
                Pacman.offsetZ += movingBy;
                Pacman.PosZ += movingBy;
                Pacman.checkDidHit();
                Pacman.checkDidEat();
                Pacman.throughTunnel();
                if (Pacman.didHit) {
                    Pacman.offsetZ -= movingBy;
                    Pacman.PosZ -= movingBy;
                }
            }
        }

        if (Pacman.didHit == FALSE) {
            if (closingMouth) {
                degreeOfPackmanMouthOpenOrClose -= 5;
            }
            else {
                degreeOfPackmanMouthOpenOrClose += 5;
            }

            if (degreeOfPackmanLives >= 360.0) {
                degreeOfPackmanLives = 0.0;
            }
            if (degreeOfPackmanMouthOpenOrClose <= -90.0) {
                closingMouth = FALSE;
            }
            if (degreeOfPackmanMouthOpenOrClose >= -45.0) {
                closingMouth = TRUE;
            }
        }
        Pacman.didHit = FALSE;
        Inky.didHit = FALSE;
        Pinky.didHit = FALSE;
        Blinky.didHit = FALSE;

        Blinky.checkDidHitPacman(Pacman);
        Inky.checkDidHitPacman(Pacman);
        Pinky.checkDidHitPacman(Pacman);
    }
    else {
        if (Pacman.dead) {
            degreeOfPackmanMouthOpenOrClose += 5;
            if (degreeOfPackmanMouthOpenOrClose >= 90) {
                cout << "lol";
                currentLives--;
                if (currentLives == 0)
                    gameOver = TRUE;
                reset();
            }
        }
        else if (Blinky.dead){
            //retreat
            if (Blinky.PosX != 0.0)
                if (Blinky.PosX > 0)
                    Blinky.PosX -= 0.01;
                else
                    Blinky.PosX += 0.01;

            if (Blinky.PosZ != 0.0)
                if (Blinky.PosZ > 0)
                    Blinky.PosZ -= 0.01;
                else
                    Blinky.PosZ += 0.01;

            // Check if the ghost has reached the cage position
            if (abs(Blinky.PosX) <= 0.02 && abs(Blinky.PosZ) <= 0.02) {
                //Blinky.scared = FALSE;
                Blinky.isRelease = FALSE;
                Blinky.dead = FALSE;
                Blinky.scaredTimer = 3;
                timer = 50 * level;
            }
        }
        else if (Inky.dead) {
            //retreat
            if (Inky.PosX != 0.0)
                if (Inky.PosX > 0)
                    Inky.PosX -= 0.01;
                else
                    Inky.PosX += 0.01;

            if (Inky.PosZ != 0.0)
                if (Inky.PosZ > 0)
                    Inky.PosZ -= 0.01;
                else
                    Inky.PosZ += 0.01;

            if (abs(Inky.PosX) <= 0.02 && abs(Inky.PosZ) <= 0.02) {
                //Inky.scared = FALSE;
                Inky.isRelease = FALSE;
                Inky.dead = FALSE;
                Inky.scaredTimer = 3;
                timer = 50 * level;
            }
        }
        else {
            if (Pinky.PosX != 0.0)
                if (Pinky.PosX > 0)
                    Pinky.PosX -= 0.01;
                else
                    Pinky.PosX += 0.01;

            if (Pinky.PosZ != 0.0)
                if (Pinky.PosZ > 0)
                    Pinky.PosZ -= 0.01;
                else
                    Pinky.PosZ += 0.01;

            if (abs(Pinky.PosX) <= 0.02 && abs(Pinky.PosZ) <= 0.02) {
                //Pinky.scared = FALSE;
                Pinky.isRelease = FALSE;
                Pinky.dead = FALSE;
                Pinky.scaredTimer = 3;
                timer = 50 * level;
            }
        }
    }

    Sleep(50 / level);
    glutPostRedisplay();
}

//Finish
void mazeModel() {
    glColor3f(0, 0, 0);
    // mid lower right horizontal wall
   
    glPushMatrix();
    glPushMatrix();
    glPushMatrix();

    //0.1 , 0, 0.0875
    //1
    glTranslatef(0.1, 0, 0.0875);
    cube(widthOfInnerWall + roadWidth, outerWallHeight,
       outerWallWidth);
    //end

    // mid right straight wall
    //0.1625, 0, -1.38778e-17
    //2
    glTranslatef(0.0625, 0, -0.0875);
    cube(outerWallWidth, outerWallHeight, widthOfInnerWall * 2 + roadWidth);
    glPopMatrix();
    // end

    // mid upper right horizontal wall
    //glPushMatrix();
    //0.2625,0,-0.0875
    //3
    glTranslatef(0.1, 0, -0.0875);
    cube(widthOfInnerWall + roadWidth, outerWallHeight,
        outerWallWidth);

    //0.4625,0,-0.25
    //4
    glTranslatef(0.2, 0, -0.1375);
    cube(widthOfInnerWall, outerWallHeight, widthOfInnerWall * 3 +
        roadWidth * 2);

    //5
    glTranslatef(-0.075, 0, 0);
    cube(widthOfInnerWall * 2 + roadWidth, outerWallHeight,
        widthOfInnerWall);

    //6
    glTranslatef(-0.225, 0, -0.075);
    cube(widthOfInnerWall, outerWallHeight,
        widthOfInnerWall * 2 + roadWidth);

    //7
    glTranslatef(0, 0, -0.075);
    cube(widthOfInnerWall * 3 + roadWidth * 2, outerWallHeight,
        widthOfInnerWall);

    //8
    glTranslatef(0, 0, -0.2375);
    cube(widthOfInnerWall, outerWallHeight, roadWidth * 2
        + outerWallWidth);

    //9
    glPushMatrix();
    glTranslatef(0, 0, -0.1);
    cube(widthOfInnerWall * 7 + roadWidth * 10 + outerWallWidth * 2
        , outerWallHeight, outerWallWidth);

    //10
    glTranslatef(0.6875, 0, 0.2375);
    cube(outerWallWidth, outerWallHeight, 2 * outerWallWidth
        + roadWidth * 4 + widthOfInnerWall);

    //11
    glTranslatef(-0.125, 0, 0.2375);
    cube(roadWidth * 2 + widthOfInnerWall + outerWallWidth,
        outerWallHeight, outerWallWidth);

    //12
    glTranslatef(-0.125, 0, 0.0875);
    cube(outerWallWidth, outerWallHeight, widthOfInnerWall
        * 2 + roadWidth);

    //13
    glTranslatef(0.125, 0, 0.0875);
    cube(roadWidth * 2 + widthOfInnerWall + outerWallWidth,
        outerWallHeight, outerWallWidth);
    glPopMatrix();

    //14
    glTranslatef(0.225, 0, 0.0625);
    cube(roadWidth + widthOfInnerWall * 2, outerWallHeight
        , roadWidth);

    //15
    glTranslatef(0.275, 0, 0);
    cube(widthOfInnerWall + roadWidth, outerWallHeight
        , roadWidth);

    //16
    glTranslatef(0, 0, 0.175);
    cube(widthOfInnerWall + roadWidth, outerWallHeight
        , widthOfInnerWall);

    glPopMatrix();

    //glRotatef(180, 0.0, 0.0, 1.0);

    glPushMatrix();

    //17
    glTranslatef(-0.1, 0, 0.0875);
    cube(widthOfInnerWall + roadWidth, outerWallHeight,
        outerWallWidth);


    //18
    glTranslatef(-0.0625, 0, -0.0875);
    cube(outerWallWidth, outerWallHeight, widthOfInnerWall * 2 + roadWidth);
    glPopMatrix();

    //19
    glTranslatef(-0.1, 0, -0.0875);
    cube(widthOfInnerWall + roadWidth, outerWallHeight,
        outerWallWidth);

    //20
    glTranslatef(-0.2025, 0, -0.1375);
    cube(widthOfInnerWall, outerWallHeight, widthOfInnerWall * 3 +
        roadWidth * 2);

    //21
    glTranslatef(0.075, 0, 0);
    cube(widthOfInnerWall * 2 + roadWidth, outerWallHeight,
        widthOfInnerWall);
    
    //22
    glTranslatef(0, 0, -0.325);
    cube(roadWidth + widthOfInnerWall * 2, outerWallHeight, roadWidth);
   
    //23
    glTranslatef(-0.275, 0, 0);
    cube(roadWidth + widthOfInnerWall, outerWallHeight, roadWidth);

    //24
    glTranslatef(0, 0, 0.175);
    cube(roadWidth + widthOfInnerWall, outerWallHeight, widthOfInnerWall);

    //25
    glTranslatef(-0.0625, 0, 0.1375);
    cube(outerWallWidth + roadWidth * 2 + widthOfInnerWall, outerWallHeight, outerWallWidth);

    //26
    glTranslatef(0, 0, 0.175);
    cube(outerWallWidth + roadWidth * 2 + widthOfInnerWall, outerWallHeight, outerWallWidth);

    //27
    glTranslatef(0.125, 0, -0.0875);
    cube(outerWallWidth, outerWallHeight, widthOfInnerWall * 2 +roadWidth);

    //28
    glTranslatef(-0.25, 0, -0.325);
    cube(outerWallWidth, outerWallHeight, outerWallWidth * 2 + roadWidth * 4 + widthOfInnerWall);

    glPopMatrix();

    glPopMatrix();
    // end

    //29
    //front wall
    glTranslatef(0, 0, 0.8125);
    cube(widthOfInnerWall * 7 + roadWidth * 10 + outerWallWidth * 2
        , outerWallHeight, outerWallWidth);

    //T joint innerWall
    //30
    glTranslatef(0, 0, -0.2125);
    cube(widthOfInnerWall, outerWallHeight,
        widthOfInnerWall * 2 + roadWidth);

    //31
    glTranslatef(0, 0, -0.075);
    cube(widthOfInnerWall * 3 + roadWidth * 2,
        outerWallHeight, widthOfInnerWall);
    //T joint innerWall end

    //save matrix to stack
    glPushMatrix();
    glPushMatrix();

    //32
    // Down right T 
    glTranslatef(0.3, 0, 0.075);
    cube(widthOfInnerWall, outerWallHeight,
        widthOfInnerWall * 2 + roadWidth);

    //33
    glTranslatef(0.05, 0, 0.075);
    cube(widthOfInnerWall * 3 + roadWidth * 3,
        outerWallHeight, widthOfInnerWall);
    // Down right T end

    //restore matrix
    glPopMatrix();

    //34
    // Down left T
    glTranslatef(-0.3, 0, 0.075);
    cube(widthOfInnerWall, outerWallHeight,
        widthOfInnerWall * 2 + roadWidth);

    //35
    glTranslatef(-0.05, 0, 0.075);
    cube(widthOfInnerWall * 3 + roadWidth * 3,
        outerWallHeight, widthOfInnerWall);
    // Down left T end

    glPopMatrix();

    //36
    //Mid low T
    glTranslatef(0, 0, -0.225);
    cube(widthOfInnerWall, outerWallHeight,
        widthOfInnerWall * 2 + roadWidth);

    glPushMatrix();

    //for meaningless recoding
    glPushMatrix();

    //37
    glTranslatef(0, 0, -0.075);
    cube(widthOfInnerWall * 3 + roadWidth * 2,
        outerWallHeight, widthOfInnerWall);

    glPopMatrix();

    //38
    //low right horizontal wall
    glTranslatef(0.225, 0, 0.075);
    cube(2 * widthOfInnerWall + roadWidth, outerWallHeight,
        widthOfInnerWall);
    //low right horizontal wall end

    glPushMatrix();

    //39
    glTranslatef(0.075, 0, -0.225);
    cube(widthOfInnerWall, outerWallHeight, 2 * widthOfInnerWall + roadWidth);

    glPopMatrix();

    //40
    // down right reverse L
    glTranslatef(0.225, 0, 0.075);

    glPushMatrix();

    cube(widthOfInnerWall, outerWallHeight, widthOfInnerWall * 2 + roadWidth);

    //41
    glTranslatef(0.05, 0, -0.075);
    cube(widthOfInnerWall + roadWidth, outerWallHeight, widthOfInnerWall);
    // down right reverse L end

    glPopMatrix();

    //42
    // down right outer wall
    glTranslatef(0.1875, 0, 0.075);
    cube(roadWidth + outerWallWidth, outerWallHeight, widthOfInnerWall);

    //43
    glTranslatef(0.05, 0, 0);
    cube(outerWallWidth, outerWallHeight, roadWidth * 4 + widthOfInnerWall * 3
        + outerWallWidth * 2);
    //  down right outer wall end

    //44
    // mid down right horizontal wall
    glTranslatef(-0.125, 0, -0.2875);
    cube(roadWidth * 2 + widthOfInnerWall + outerWallWidth, outerWallHeight
        , outerWallWidth);
    // mid down right horizontal wall end

    //45
    // mid right straight line
    glTranslatef(-0.125, 0, -0.0875);
    cube(outerWallWidth, outerWallHeight, widthOfInnerWall * 2 + roadWidth);
    // mid right straight line end

    //46
    // mid up right horizontal wall
    glTranslatef(0.125, 0, -0.0875);
    cube(roadWidth * 2 + widthOfInnerWall + outerWallWidth, outerWallHeight
        , outerWallWidth);
    // mid up right horizontal wall end

    //Rotate everthing
    glPopMatrix();

    //glRotatef(180, 0.0, 0.0, 1.0);
    
    //47
    glTranslatef(-0.235, 0, 0.075);
    cube(2 * widthOfInnerWall + roadWidth, outerWallHeight,
        widthOfInnerWall);

    //48
    glTranslatef(-0.275, 0, 0);
    cube(roadWidth + widthOfInnerWall, outerWallHeight, widthOfInnerWall);

    //49
    glTranslatef(0.05, 0, 0.075);
    cube(widthOfInnerWall, outerWallHeight, widthOfInnerWall * 2 + roadWidth);

    //50
    glTranslatef(-0.18125, 0, 0.075);
    cube(outerWallWidth + roadWidth, outerWallHeight, widthOfInnerWall);
    //glPushMatrix();

    //51
    glTranslatef(-0.05, 0, 0);
    cube(outerWallWidth, outerWallHeight, roadWidth * 4 + widthOfInnerWall * 3 + outerWallWidth * 2);

    //52
    glTranslatef(0.125, 0, -0.2875);
    cube(outerWallWidth + roadWidth * 2 + widthOfInnerWall, outerWallHeight, outerWallWidth);

    //53
    glTranslatef(0.125, 0, -0.0875);
    cube(outerWallWidth, outerWallHeight, widthOfInnerWall * 2 + roadWidth);
    glPushMatrix();

    //54
    glTranslatef(0.1375, 0, 0);
    cube(widthOfInnerWall, outerWallHeight, widthOfInnerWall * 2 + roadWidth);
    
    glPopMatrix();

    //55
    glTranslatef(-0.125, 0, -0.0875);
    cube(outerWallWidth + roadWidth * 2 + widthOfInnerWall, outerWallHeight, outerWallWidth);
    
    glMatrixMode(GL_MODELVIEW);
    glLoadIdentity();
    if (viewMode == 1)
    {
        gluLookAt(Pacman.PosX + forThridPersonX, 0.25, Pacman.PosZ + forThridPersonZ, Pacman.PosX, 0, Pacman.PosZ, 0, 1, 0);
    }
    else if (viewMode == 2) {
        gluLookAt(Pacman.PosX + 1 * cos(cameraRotationBy), 1,
            Pacman.PosZ + 1 * sin(cameraRotationBy), Pacman.PosX, 0, Pacman.PosZ, 0, 1, 0);
    }
    else if (viewMode == 3) {
        gluLookAt(0.0, 2.5, 0.0, 0, 0, 0, 0, 0, -1);
        glRotatef(mapRotatedBy, 0, 1, 0);
    }
    else if (viewMode == 4) {
        //gluLookAt(-0.5, 0.1, 0.5, -0.5, 0.05, -0.9125, 0, 1, 0);
        gluLookAt(1.5, 1.5, 1.5, 0, 0, 0, 0, 1, 0);
        glRotatef(mapRotatedBy, 0, 1, 0);
    }
}

void ground() {
    glPushMatrix();
    glEnable(GL_TEXTURE_2D);
    glBindTexture(GL_TEXTURE_2D, 1);
    glTranslatef(0, -outerWallHeight + 0.0175, 0.05);
    glBegin(GL_QUADS);
    glTexCoord2f(0.0f, 1.0f); glVertex3f(-0.675f, 0.0f, -0.775f);
    glTexCoord2f(1.0f, 1.0f); glVertex3f(0.675f, 0.0f, -0.775f);
    glTexCoord2f(1.0f, 0.0f); glVertex3f(0.675f, 0.0f, 0.775f);
    glTexCoord2f(0.0f, 0.0f); glVertex3f(-0.675f, 0.0f, 0.775f);
    glEnd();
    glColor3f(255, 255, 0);
    glDisable(GL_TEXTURE_2D);
    glPopMatrix();
}

void spawnPacman() {
    //pac-man
    glPushMatrix();
    glMaterialfv(GL_FRONT_AND_BACK, GL_SPECULAR, PacmanSpecular);
    glMaterialfv(GL_FRONT_AND_BACK, GL_SHININESS, PacmanShininess);
    glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT_AND_DIFFUSE, PacmanAmbientDiffuse);
    glMaterialfv(GL_FRONT_AND_BACK, GL_SPECULAR, PacmanSpecular);
    glMaterialfv(GL_FRONT_AND_BACK, GL_EMISSION, PacmanEmission);
    // TODO: Pacman move
    //cout << offsetX << " " << offsetZ << endl;
    glTranslatef(Pacman.PosX, 0.025, Pacman.PosZ);
    glRotatef(Pacman.facingRotationBy, 0, 1, 0);
    glColor3f(1, 1, 0);
    wireCube(0.1, 0.1, 0.1);
    sphere(pacmanRadius / 2, degreeOfPackmanMouthOpenOrClose);
    //cout << Pacman.PosX << " " << Pacman.PosZ << endl;
    glPopMatrix();
}

void spawnGhost() {
    glPushMatrix();
    ghostPos[0][0] = Blinky.PosX;
    ghostPos[0][1] = Blinky.PosZ;
    ghostPos[0][2] = Blinky.scared;
    ghostPos[0][3] = Blinky.dead;
    ghostPos[1][0] = Pinky.PosX;
    ghostPos[1][1] = Pinky.PosZ;
    ghostPos[1][2] = Pinky.scared;
    ghostPos[1][3] = Pinky.dead;
    ghostPos[2][0] = Inky.PosX;
    ghostPos[2][1] = Inky.PosZ;
    ghostPos[2][2] = Inky.scared;
    ghostPos[2][3] = Inky.dead;
    for (int i = 0; i < 3; i++) {
        glMaterialfv(GL_FRONT, GL_SPECULAR, GhostSpecular);
        glMaterialfv(GL_FRONT, GL_SHININESS, GhostShinniness);
        if(ghostPos[i][2])
            glMaterialfv(GL_FRONT, GL_EMISSION, GhostEmision[3]);
        else
            glMaterialfv(GL_FRONT, GL_EMISSION, GhostEmision[i]);
        glMaterialfv(GL_FRONT, GL_AMBIENT_AND_DIFFUSE, GhostDiffuse[i]);
        if (viewMode != 1 && viewMode != 3) {
            glPushMatrix();
            glColor3f(ghostRGB[i][0], ghostRGB[i][1], ghostRGB[i][2]);
            glTranslatef(ghostPos[i][0], 0.05, ghostPos[i][1]);
            if (ghostPos[i][3] == 0)
                glutSolidSphere(0.5 / devideRadiusBy, 30, 30);
            GLUquadricObj* p = gluNewQuadric();
            glPushMatrix();
            glPushMatrix();
            glRotatef(90, 1, 0, 0);
            if (ghostPos[i][3] == 0) {
                wireCube(0.1, 0.1, 0.1);
                gluCylinder(p, 0.5 / devideRadiusBy, 0.5 / devideRadiusBy, 0.5 / devideRadiusBy, 40, 40);
            }
            glPopMatrix();
            glDisable(GL_LIGHTING);
            glColor3f(1, 1, 1);
            glEnable(GL_TEXTURE_2D);
            if (ghostPos[i][2] == 0) {
                glBindTexture(GL_TEXTURE_2D, 21);
            }
            else {
                glBindTexture(GL_TEXTURE_2D, 23);
            }
            glEnable(GL_TEXTURE_GEN_S);
            glEnable(GL_TEXTURE_GEN_T);
            glTexGeni(GL_S, GL_TEXTURE_GEN_MODE, GL_SPHERE_MAP);
            glTexGeni(GL_T, GL_TEXTURE_GEN_MODE, GL_SPHERE_MAP);
            glTranslatef(-0.015, 0, 0.25 / devideRadiusBy);
            glutSolidSphere(0.25 / devideRadiusBy, 30, 30);
            if (ghostPos[i][2] == 0) {
                glBindTexture(GL_TEXTURE_2D, 22);
            }
            else {
                glBindTexture(GL_TEXTURE_2D, 24);
            }
            glTranslatef(0.03, 0, 0);
            glutSolidSphere(0.25 / devideRadiusBy, 30, 30);
            glDisable(GL_TEXTURE_GEN_S);
            glDisable(GL_TEXTURE_GEN_T);
            glDisable(GL_TEXTURE_2D);
            glEnable(GL_LIGHTING);
            glPopMatrix();
            glColor3f(ghostRGB[i][0], ghostRGB[i][1], ghostRGB[i][2]);
            glRotatef(degreeOfPackmanLives, 0, 1, 0);
            if (ghostPos[i][3] == 0) {
                for (int j = 0; j < n_legs; j++) {
                    glPushMatrix();
                    glTranslatef(0, -0.5 / devideRadiusBy, 0);
                    glRotatef(360.0f * j / n_legs, 0, 1, 0);
                    glTranslatef(0.405 / devideRadiusBy, 0, 0);
                    glRotatef(90, 1, 0, 0);
                    glutSolidCone(0.1 / devideRadiusBy, 0.2 / devideRadiusBy, 10, 10);
                    glPopMatrix();
                }
            }
            glPopMatrix();
        }
        else {
            glPushMatrix();
            glColor3f(ghostRGB[i][0], ghostRGB[i][1], ghostRGB[i][2]);
            glTranslatef(ghostPos[i][0], 0.05, ghostPos[i][1]);
            if (ghostPos[i][3] == 0)
                glutSolidSphere(0.5 / devideRadiusBy, 30, 30);
            GLUquadricObj* p = gluNewQuadric();
            glPushMatrix();
            glPushMatrix();
            glRotatef(90, 1, 0, 0);
            if (ghostPos[i][3] == 0) {
                wireCube(0.1, 0.1, 0.1);
                gluCylinder(p, 0.5 / devideRadiusBy, 0.5 / devideRadiusBy, 0.5 / devideRadiusBy, 40, 40);
            }
            glPopMatrix();
            glDisable(GL_LIGHTING);
            glColor3f(1, 1, 1);
            glTranslatef(-0.015, 0, 0.25 / devideRadiusBy);
            glPushMatrix();
            glutSolidSphere(0.25 / devideRadiusBy, 30, 30);
            glTranslatef(-0.005, 0, (0.25 / devideRadiusBy) / 2 + 0.003);
            glColor3f(0, 0, 0);
            glutSolidSphere((0.25 / devideRadiusBy) / 2, 30, 30);
            glPopMatrix();
            glColor3f(1, 1, 1);
            glTranslatef(0.03, 0, 0);
            glutSolidSphere(0.25 / devideRadiusBy, 30, 30);
            glTranslatef(0.005, 0, (0.25 / devideRadiusBy) / 2 + 0.003);
            glColor3f(0, 0, 0);
            glutSolidSphere((0.25 / devideRadiusBy) / 2, 30, 30);
            glPopMatrix();
            glEnable(GL_LIGHTING);
            glColor3f(ghostRGB[i][0], ghostRGB[i][1], ghostRGB[i][2]);
            glRotatef(degreeOfPackmanLives, 0, 1, 0);
            if (ghostPos[i][3] == 0) {
                for (int j = 0; j < n_legs; j++) {
                    glPushMatrix();
                    glTranslatef(0, -0.5 / devideRadiusBy, 0);
                    glRotatef(360.0f * j / n_legs, 0, 1, 0);
                    glTranslatef(0.405 / devideRadiusBy, 0, 0);
                    glRotatef(90, 1, 0, 0);
                    glutSolidCone(0.1 / devideRadiusBy, 0.2 / devideRadiusBy, 10, 10);
                    glPopMatrix();
                }
            }
            glPopMatrix();
        }
    }
    glPopMatrix();
}

//Finish
void spawnFood() {
    glMaterialfv(GL_FRONT_AND_BACK, GL_SPECULAR, PacmanSpecular);
    glMaterialfv(GL_FRONT, GL_SHININESS, PacmanShininess);
    glMaterialfv(GL_FRONT, GL_EMISSION, FoodEmission);
    glMaterialfv(GL_FRONT, GL_AMBIENT_AND_DIFFUSE, PacmanDiffuse);
    for (int i = 0; i < sizeof(foodPos) / sizeof(foodPos[0]); i++) {
        if (!eaten[i][0]) {
            glPushMatrix();
            glTranslatef(foodPos[i][0], 0, foodPos[i][1]);
            glutSolidSphere(pacmanRadius / 6, 10, 10);
            glPopMatrix();
        }
    }

    for (int i = 0; i < sizeof(bigFoodPos) / sizeof(bigFoodPos[0]); i++) {
        if (!eaten[sizeof(foodPos) / sizeof(foodPos[0]) + i][0]) {
            glPushMatrix();
            glTranslatef(bigFoodPos[i][0], 0, bigFoodPos[i][1]);
            glutSolidSphere(pacmanRadius / 4, 10, 10);
            glPopMatrix();
        }
    }
}

//FINISH
void calWallsBound() {
    for (int i = 0; i < sizeof(myWallPos) / sizeof(myWallPos[0]); i++) {
        //X-min
        eachWallBound[i][0] = myWallPos[i][0] - wallHWL[i][0] / 2.0;
        //X-max
        eachWallBound[i][1] = myWallPos[i][0] + wallHWL[i][0] / 2.0;
        //Z-max
        eachWallBound[i][2] = myWallPos[i][1] + wallHWL[i][1] / 2.0;
        //Z-min
        eachWallBound[i][3] = myWallPos[i][1] - wallHWL[i][1] / 2.0;

        cout << eachWallBound[i][0] << " " << eachWallBound[i][1]
        << " " << eachWallBound[i][2] << " " << eachWallBound[i][3]
        << endl;
    }
}

void displayLivesModel() {
    glDisable(GL_LIGHTING);
    glLineWidth(1.0f);
    glColor3f(1, 0, 0);
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    glMatrixMode(GL_MODELVIEW);
    glLoadIdentity();
    gluOrtho2D(-1, 1, -1, 1);

    if (gameOver) {
        glutIdleFunc(NULL);
        glColor3f(1, 1, 1);
        glPushMatrix();
        glEnable(GL_TEXTURE_2D);
        glBindTexture(GL_TEXTURE_2D, 25);
        glBegin(GL_POLYGON);
        glTexCoord2f(0.0f, 1.0f); glVertex2f(-0.5, 0.5);
        glTexCoord2f(0.0f, 0.0f); glVertex2f(-0.5, -0.5);
        glTexCoord2f(1.0f, 0.0f); glVertex2f(0.5, -0.5);
        glTexCoord2f(1.0f, 1.0f); glVertex2f(0.5, 0.5);
        glEnd();
        glPopMatrix();
        glDisable(GL_TEXTURE_2D);
    }

    glColor3f(1, 0, 0);

    glTranslatef(-0.9, -0.9, 0);

    //L in Lives
    glBegin(GL_LINE_LOOP);
    glVertex2f(0.05, 0.05);
    glVertex2f(0.05, 0.25);
    glVertex2f(0.075, 0.25);
    glVertex2f(0.075, 0.075);
    glVertex2f(0.15, 0.075);
    glVertex2f(0.15, 0.05);
    glEnd();

    //I
    glBegin(GL_LINE_LOOP);
    glVertex2f(0.2, 0.05);
    glVertex2f(0.2, 0.075);
    glVertex2f(0.225, 0.075);
    glVertex2f(0.225, 0.225);
    glVertex2f(0.2, 0.225);
    glVertex2f(0.2, 0.25);
    glVertex2f(0.275, 0.25);
    glVertex2f(0.275, 0.225);
    glVertex2f(0.25, 0.225);
    glVertex2f(0.25, 0.075);
    glVertex2f(0.275, 0.075);
    glVertex2f(0.275, 0.05);
    glEnd();

    //V
    glBegin(GL_LINE_LOOP);
    glVertex2f(0.325, 0.25);
    glVertex2f(0.35, 0.25);
    glVertex2f(0.325 + 0.1 / 2, 0.075);
    glVertex2f(0.325 + 0.1 - 0.025, 0.25);
    glVertex2f(0.325 + 0.1, 0.25);
    glVertex2f((0.325 + 0.1 / 2) + ((0.1 / 2
        - 0.025) / 2), 0.05);
    glVertex2f((0.325 + 0.1 / 2) - ((0.1 / 2
        - 0.025) / 2), 0.05);
    glEnd();

    glBegin(GL_LINE_LOOP);
    glVertex2f(0.475, 0.05);
    glVertex2f(0.575, 0.05);
    glVertex2f(0.575, 0.075);
    glVertex2f(0.5, 0.075);
    glVertex2f(0.5, (0.075 + (0.2 - 0.025 * 3) / 2));
    glVertex2f(0.575, (0.075 + (0.2 - 0.025 * 3) / 2));
    glVertex2f(0.575, (0.075 + (0.2 - 0.025 * 3) / 2)
        + 0.025);
    glVertex2f(0.5, (0.075 + (0.2 - 0.025 * 3) / 2)
        + 0.025);
    glVertex2f(0.5, (0.05 + 0.2 - 0.025));
    glVertex2f(0.575, (0.05 + 0.2 - 0.025));
    glVertex2f(0.575, (0.05 + 0.2));
    glVertex2f(0.475, (0.05 + 0.2));
    glEnd();

    glColor3f(255, 255, 0);
    for (int i = currentLives - 1; i >= 0; i--)
    {
        glLoadIdentity();
        gluOrtho2D(-1, 1, -1, 1);
        glTranslatef(-0.025 + i * 0.3, -0.75, 0);
        glRotatef(degreeOfPackmanLives, 0, 1, 0);
        sphere(pacmanRadius, -45);
    }

    glColor3f(1.0, 1.0, 1.0);
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluPerspective(45, 1, 0.1, 100);
    glEnable(GL_LIGHTING);
}

void directionGuide() {
    glPushMatrix();
    // "w"
    glTranslatef(Pacman.PosX - 0.05, 0, Pacman.PosZ - 0.05);
    glRotatef(-90, 1, 0, 0);
    glScalef(0.2, 0.2, 0);
    glLineWidth(7.0f);
    glBegin(GL_LINES);
    glVertex2f(0.1, 0.4);
    glVertex2f(0.2, 0.1);
    glVertex2f(0.2, 0.1);
    glVertex2f(0.3, 0.4);
    glVertex2f(0.3, 0.4);
    glVertex2f(0.4, 0.1);
    glVertex2f(0.4, 0.1);
    glVertex2f(0.5, 0.4);
    glEnd();
    glPopMatrix();

    glPushMatrix();
    // A
    glTranslatef(Pacman.PosX - 0.175, 0, Pacman.PosZ + 0.05);
    glRotatef(-90, 1, 0, 0);
    glScalef(0.2, 0.2, 0);
    glBegin(GL_LINES);
    glVertex2f(0.1, 0.1);
    glVertex2f(0.2, 0.4);
    glVertex2f(0.2, 0.4);
    glVertex2f(0.3, 0.1);
    glEnd();

    glBegin(GL_LINES);
    glVertex2f(0.25, 0.25);
    glVertex2f(0.15, 0.25);
    glEnd();
    glPopMatrix();

    glLineWidth(1.0f);
}

void pauseScreen() {
    glDisable(GL_LIGHTING);
    glDisable(GL_DEPTH_TEST);
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    glMatrixMode(GL_MODELVIEW);
    glLoadIdentity();
    gluOrtho2D(-1, 1, -1, 1);

    glBegin(GL_POLYGON);
    glVertex2f(0, -0.5);
    glVertex2f(0.5, 0);
    glVertex2f(0, 0.5);
    glVertex2f(-0.5, 0);
    glEnd();

    glColor3f(0.0, 0.0, 0.0);
    glBegin(GL_POLYGON);
    glVertex2f(-0.05, -0.15);
    glVertex2f(-0.1, -0.15);
    glVertex2f(-0.1, 0.15);
    glVertex2f(-0.05, 0.15);
    glEnd();

    glBegin(GL_POLYGON);
    glVertex2f(0.05, 0.15);
    glVertex2f(0.1, 0.15);
    glVertex2f(0.1, -0.15);
    glVertex2f(0.05, -0.15);
    glEnd();

    glColor3f(1.0, 1.0, 1.0);
    glViewport(0, 0, glutGet(GLUT_WINDOW_WIDTH), glutGet(GLUT_WINDOW_HEIGHT));
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluPerspective(45, 1, -0.5, 100);
    glEnable(GL_LIGHTING);
}

void pointsModel() {
    glPushMatrix();
    glTranslatef(-0.5, 0.05, -0.9125);
    glColor3f(112, 128, 144);
    cube(0.8, 0.05, 0.3);
    glTranslatef(-0.375, 0.1, 0);
    cube(0.05, 0.25, 0.3);
    glTranslatef(0.375, 0.1, 0);
    cube(0.8, 0.05, 0.3);
    glPushMatrix();
    glColor3f(255, 255, 0);

    while (checkPointsSize >= 10) {
        checkPointsSize /= 10;
        divisor *= 10;
    }

    glTranslatef(-0.275, 0, 0.15);
    int i = 0;
    while (divisor >= 1) {
        int digit = (points / divisor) % 10;

        glPushMatrix();
        glTranslatef(0.15 * i, 0, 0);
        //cout << digit << endl;
        drawNumber(digit);
        glPopMatrix();

        divisor /= 10;
        i++;
    }

    checkPointsSize = points;
    divisor = 1;
    glPopMatrix();
    glColor3f(112, 128, 144);
    glTranslatef(0.375, -0.1, 0);
    cube(0.05, 0.25, 0.3);
    glPopMatrix();
}

void display() {
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
    glEnable(GL_DEPTH_TEST);
    glMatrixMode(GL_MODELVIEW);
    glLoadIdentity();

    //background
    glDisable(GL_LIGHTING);
    glDisable(GL_LIGHT0);
    glMatrixMode(GL_PROJECTION);
    glPushMatrix();
    glLoadIdentity();
    gluOrtho2D(-1, 1, -1, 1);
    glMatrixMode(GL_MODELVIEW);
    glLoadIdentity();
    glDisable(GL_DEPTH_TEST);

    glBindTexture(GL_TEXTURE_2D, imageFrame);
    glEnable(GL_TEXTURE_2D);
    glBegin(GL_QUADS);
    glTexCoord2f(0.0f, 0.0f); glVertex2f(-1.0f, -1.0f);
    glTexCoord2f(1.0f, 0.0f); glVertex2f(1.0f, -1.0f);
    glTexCoord2f(1.0f, 1.0f); glVertex2f(1.0f, 1.0f);
    glTexCoord2f(0.0f, 1.0f); glVertex2f(-1.0f, 1.0f);
    glEnd();
    glDisable(GL_TEXTURE_2D);

    glEnable(GL_DEPTH_TEST);
    glMatrixMode(GL_PROJECTION);
    glPopMatrix();
    glMatrixMode(GL_MODELVIEW);

    //45 degree to 0.0.0 coord
    // to lookat packman
    // camera follow
    // WIP
    if (viewMode == 1)
    {
        gluLookAt(Pacman.PosX + forThridPersonX, 0.25,
            Pacman.PosZ + forThridPersonZ, Pacman.PosX, 0, Pacman.PosZ, 0, 1, 0);
    }
    else if (viewMode == 2) {
        gluLookAt(Pacman.PosX + (1 - zoomValue) * cos(cameraRotationBy),
            1 - zoomValue, Pacman.PosZ + (1 - zoomValue) * sin(cameraRotationBy),
            Pacman.PosX, 0, Pacman.PosZ, 0, 1, 0);
    }
    else if (viewMode == 3) {
        gluLookAt(0.0, 2.5 - zoomValue, 0.0, 0, 0, 0, 0, 0, -1);
        glRotatef(mapRotatedBy, 0, 1, 0);
    }
    else if (viewMode == 4) {
        //gluLookAt(-0.5, 0.1, 0.5, -0.5, 0.05, -0.9125, 0, 1, 0);
        gluLookAt(1.5 - zoomValue, 1.5 - zoomValue,
            1.5 - zoomValue, 0, 0, 0, 0, 1, 0);
        glRotatef(mapRotatedBy, 0, 1, 0);
    }
    glEnable(GL_LIGHTING);
    glEnable(GL_LIGHT0);
    glPushMatrix();
        glRotated(degreeOfPackmanLives, 1.0, 0.0, 0.0);
        glLightfv(GL_LIGHT0, GL_AMBIENT, light0Ambient);
        glLightfv(GL_LIGHT0, GL_DIFFUSE, diffuse);
        glLightfv(GL_LIGHT0, GL_POSITION, light0Position);
        glLightfv(GL_LIGHT0, GL_SPOT_DIRECTION, light0Direction);
        glLightfv(GL_LIGHT0, GL_SPOT_CUTOFF, light0Cutoff);
        glLightfv(GL_LIGHT0, GL_SPOT_EXPONENT, light0Exponent);
        glEnable(GL_LIGHTING);
    glPopMatrix();

    ground();

    directionGuide();

    spawnPacman();

    spawnFood();

    spawnGhost();

    pointsModel();

    mazeModel();

    displayLivesModel();

    if (pause == TRUE) {
        pauseScreen();
    }

    glFlush();
    glutSwapBuffers();
}

int main(int argc, char** argv) {
    cout << myWallPos[24][0] << " " << myWallPos[24][1] << endl;
    for (int i = 1; i < 40; i++) {
        cout << "{ " << foodPos[sizeof(foodPos) / sizeof(foodPos[0]) - 1 ][0] << ","
            << foodPos[sizeof(foodPos) / sizeof(foodPos[0]) - 1][1] - i * 0.05 << " }," << endl;
    }

    //Every food not eaten
    for (int i = 0; i < sizeof(foodPos) / sizeof(foodPos[0])
        + sizeof(bigFoodPos) / sizeof(bigFoodPos[0]); i++) {
        eaten[i][0] = FALSE;
    }

    //initiallize Pacman
    Pacman.PosX = 0;
    Pacman.PosZ = (widthOfInnerWall * 2 + roadWidth) / 2.0 + roadWidth / 2.0;
    Pacman.facingRotationBy = 0;
    cout << Pacman.offsetX << " " << Pacman.offsetZ << endl;

    //initiallize ghost
    Blinky.PosX = 0.085;
    Blinky.PosZ = 0;

    Pinky.PosX = Blinky.PosX - 0.09;
    Pinky.PosZ = 0;

    Inky.PosX = Pinky.PosX - 0.09;
    Inky.PosZ = 0;

    //initiallize wallBound
    calWallsBound();
    
    cout << Pacman.offsetX << " " << Pacman.offsetZ << endl;

    glutInit(&argc, argv);
    glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB);
    glutInitWindowSize(800, 800);
    glutCreateWindow("PacmanGameProject");

    initGL();
    glutDisplayFunc(display);
    glutIdleFunc(idleHandler);
    glutKeyboardFunc(keyBoardHandler);
    glutSpecialFunc(specialKeyHandler);
    glutReshapeFunc(windowChangingPosHandler);
    glutMainLoop();
    return 0;
}