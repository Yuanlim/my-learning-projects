from flask import Flask, request
import requests
import pandas as pd
from bs4 import BeautifulSoup
import math
import random

# TODO: change the default Entry Point text to handleWebhook

app = Flask(__name__)


@app.route('/webhook', methods=['GET', 'POST'])
def handleWebhook():
    print("receive a request")
    req = request.get_json(force=True)
    responseText = ""
    intent = req["queryResult"]["intent"]["displayName"]
    myWeatherAPIKey = "------------------"
    if intent == "AskForWeather":
        try:
            countryName = req["queryResult"]["parameters"]["location"]["country"]
            myCountryTranslateDataSetDir = "myCountryTranslateDataSet.xlsx"
            countryToEN = translateFunc(myCountryTranslateDataSetDir, countryName)
            url = "https://api.openweathermap.org/data/2.5/weather?q=" + f"{countryToEN}" + "&lang=zh_tw&units=metric&appid=" + f"{myWeatherAPIKey}"
            weatherAPIResponse = requests.get(url)  # weatherAPI http request
            weatherJsonData = weatherAPIResponse.json()  # get web json
            tempature = weatherJsonData["main"]["temp"]
            weather = weatherJsonData["weather"][0]["description"]
            responseText = f"{countryName}的天氣為{tempature}°C, {weather}"
        except:
            url = "https://api.openweathermap.org/data/2.5/weather?q=taiwan&units=metric&lang=zh_tw&appid="+f"{myWeatherAPIKey}"  # set url format
            weatherAPIResponse = requests.get(url)  # weatherAPI http request
            weatherJsonData = weatherAPIResponse.json()  # get web json
            tempature = weatherJsonData["main"]["temp"]  # extract information from json
            weather = weatherJsonData["weather"][0]["description"]  # ..
            responseText = f"台灣的天氣為{tempature}°C, {weather}"
    # Product info
    elif intent == "CustomerInterest_None(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            price = dataSet["Price"][index]
            spec = dataSet["Specs"][index]
            haveOfferOrNot = dataSet["HaveOfferOrNot"][index]
            warranty = dataSet["Warranty"][index]
            links = dataSet["Links"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
有出售的地方：{whereThatHaveSales}
售價：{price}
規格：
{spec}
有優惠嗎？：{haveOfferOrNot}
有保固嗎？：{warranty}
購買鏈接：{links}
                            """
    elif intent == "CustomerInterest_P(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            price = dataSet["Price"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
售價為{price}"""
    elif intent == "CustomerInterest_W(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            links = dataSet["Links"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
出售地為{whereThatHaveSales},購買網址：{links}"""
    elif intent == "CustomerInterest_S(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            spec = dataSet["Specs"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
規格為：
{spec}
                            """
    elif intent == "CustomerInterest_O(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 是否有優惠?：{Offer}"""
    elif intent == "CustomerInterest_OP(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            price = dataSet["Price"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 是否有優惠?：{Offer} 價格為:{price}"""
    elif intent == "CustomerInterest_OPS(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            price = dataSet["Price"][index]
            spec = dataSet["Specs"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer} 
價格為：{price}
規格為：{spec}"""
    elif intent == "CustomerInterest_OPSW(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            price = dataSet["Price"][index]
            spec = dataSet["Specs"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer} 
價格為：{price}
規格為：{spec}
哪裡能買到？：{whereThatHaveSales}"""
    elif intent == "CustomerInterest_OPW(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            price = dataSet["Price"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer} 
價格為：{price}
哪裡能買到？：{whereThatHaveSales}"""
    elif intent == "CustomerInterest_OPWWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            price = dataSet["Price"][index]
            Warranty = dataSet["Warranty"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer} 
價格為：{price}
哪裡能買到？：{whereThatHaveSales}
保固期限：{Warranty}"""
    elif intent == "CustomerInterest_OS(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            spec = dataSet["Specs"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer} 
規格為：{spec}"""
    elif intent == "CustomerInterest_OSW(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            spec = dataSet["Specs"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer} 
規格為：{spec}
哪裡能買到：{whereThatHaveSales}"""
    elif intent == "CustomerInterest_OSWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            spec = dataSet["Specs"][index]
            Warranty = dataSet["Warranty"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer} 
規格為：{spec}
保固期限：{Warranty}"""
    elif intent == "CustomerInterest_OSWWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            spec = dataSet["Specs"][index]
            Warranty = dataSet["Warranty"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer} 
規格為：{spec}
哪裡能買到：{whereThatHaveSales}
保固期限：{Warranty}"""
    elif intent == "CustomerInterest_OW(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer}
哪裡能買到：{whereThatHaveSales}"""
    elif intent == "CustomerInterest_OWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            Warranty = dataSet["Warranty"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer}
保固期限：{Warranty}"""
    elif intent == "CustomerInterest_OWWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Offer = dataSet["HaveOfferOrNot"][index]
            spec = dataSet["Specs"][index]
            Warranty = dataSet["Warranty"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 
是否有優惠?：{Offer} 
哪裡能買到：{whereThatHaveSales}
保固期限：{Warranty}"""
    elif intent == "CustomerInterest_War(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            Warranty = dataSet["Warranty"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM 保固期限：{Warranty}"""
    elif intent == "CustomerInterest_PW(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            where = dataSet["WhereThatHaveSales"][index]
            price = dataSet["Price"][index]
            link = dataSet["Links"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
價格：{price}
哪裡有出售？：{where}
購買網址：{link}
                            """
    elif intent == "CustomerInterest_PS(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            price = dataSet["Price"][index]
            Spec = dataSet["Specs"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
價格：{price}
產品規格：
{Spec}
                            """
    elif intent == "CustomerInterest_PWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            price = dataSet["Price"][index]
            Warranty = dataSet["Warranty"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
價格：{price}
保固期限：{Warranty}
                                    """
    elif intent == "CustomerInterest_SW(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            spec = dataSet["Specs"][index]
            links = dataSet["Links"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
有出售的地方：{whereThatHaveSales}
規格：
{spec}
購買鏈接：{links}
                                    """
    elif intent == "CustomerInterest_WWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            price = dataSet["Price"][index]
            spec = dataSet["Specs"][index]
            haveOfferOrNot = dataSet["HaveOfferOrNot"][index]
            warranty = dataSet["Warranty"][index]
            links = dataSet["Links"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
有出售的地方：{whereThatHaveSales}
有保固嗎？：{warranty}
購買鏈接：{links}
                                    """
    elif intent == "CustomerInterest_SWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            spec = dataSet["Specs"][index]
            warranty = dataSet["Warranty"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
規格：
{spec}
有保固嗎？：{warranty}
                                    """
    elif intent == "CustomerInterest_PSW(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            price = dataSet["Price"][index]
            spec = dataSet["Specs"][index]
            links = dataSet["Links"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
有出售的地方：{whereThatHaveSales}
售價：{price}
規格：
{spec}
購買鏈接：{links}
                                    """
    elif intent == "CustomerInterest_PWWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            price = dataSet["Price"][index]
            warranty = dataSet["Warranty"][index]
            links = dataSet["Links"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
有出售的地方：{whereThatHaveSales}
售價：{price}
有保固嗎？：{warranty}
購買鏈接：{links}
                                    """
    elif intent == "CustomerInterest_PSWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            price = dataSet["Price"][index]
            spec = dataSet["Specs"][index]
            warranty = dataSet["Warranty"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
售價：{price}
規格：
{spec}
有保固嗎？：{warranty}"""
    elif intent == "CustomerInterest_PSWWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            price = dataSet["Price"][index]
            spec = dataSet["Specs"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            warranty = dataSet["Warranty"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
售價：{price}
規格：
{spec}
有保固嗎？：{warranty}
哪裡能買到：{whereThatHaveSales}"""
    elif intent == "CustomerInterest_SWWar(Phone)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        returnResponse = intentPhoneProduct(req)
        index = returnResponse[0]
        if returnResponse[1] != "":
            responseText = returnResponse[1]
        else:
            productName = dataSet["ProductName"][index]
            ROM = dataSet["ROM"][index]
            SSD = dataSet["SSD"][index]
            RAM = dataSet["RAM"][index]
            whereThatHaveSales = dataSet["WhereThatHaveSales"][index]
            spec = dataSet["Specs"][index]
            warranty = dataSet["Warranty"][index]
            links = dataSet["Links"][index]
            responseText = f"""使用者問的產品：{productName},{ROM} ROM,{SSD} SSD,{RAM} RAM
有出售的地方：{whereThatHaveSales}
規格：
{spec}
有保固嗎？：{warranty}
購買鏈接：{links}
                                    """
    elif intent == "AskForLowestPriceProduct":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)

        Brands, productBrandsCol, ProductTpye, ProductTpyeCol = extractingJsonReqProductTypeAndBrands(req, dataSet, isFallback=False)

        priceIndexArray = [[]]
        for i in range(len(dataSet)):
            priceString = dataSet["Price"][i]
            try:
                math.isnan(dataSet["產品公司"][i])
            except:
                if productBrandsCol[i] == Brands and ProductTpyeCol[i] == ProductTpye:
                    firstMatch = i
                    if priceString.find("/") != -1:
                        StartPos = -1
                        EndPos = -1
                        price = []
                        for j in range(len(priceString)):
                            if j > EndPos:
                                try:
                                    tryIntChar = int(priceString[j])
                                    StartPos = j
                                    while True:
                                        j += 1
                                        tryIntChar = int(priceString[j])
                                except:
                                    if StartPos != -1:
                                        EndPos = j
                                        price.append(int(priceString[StartPos:EndPos]))
                                        StartPos = -1
                        priceIndexArray[0].append(min(price))
                        priceIndexArray[0].append(i)
                    else:
                        priceIndexArray[0].append(int(priceString[1:]))
                        priceIndexArray[0].append(i)
                    break

        if priceIndexArray[0][0] != "":
            for i in range(firstMatch+1, len(dataSet)):
                lastIndex = len(priceIndexArray)
                if productBrandsCol[i] != "" and productBrandsCol[i] == Brands and ProductTpyeCol[i] == ProductTpye:
                    priceIndexArray.append([])
                    priceString = dataSet["Price"][i]
                    if priceString.find("/") != -1:
                        StartPos = -1
                        EndPos = -1
                        price = []
                        for j in range(len(priceString)):
                            if j > EndPos:
                                try:
                                    tryIntChar = int(priceString[j])
                                    StartPos = j
                                    while True:
                                        j += 1
                                        tryIntChar = int(priceString[j])
                                except:
                                    if StartPos != -1:
                                        EndPos = j
                                        price.append(int(priceString[StartPos:EndPos]))
                                        StartPos = -1
                        priceIndexArray[lastIndex].append(min(price))
                        priceIndexArray[lastIndex].append(i)
                    else:
                        priceIndexArray[lastIndex].append(int(priceString[1:]))
                        priceIndexArray[lastIndex].append(i)
                    while priceIndexArray[lastIndex][0] < priceIndexArray[lastIndex-1][0] and lastIndex > 0:
                        temp = priceIndexArray[lastIndex-1]
                        priceIndexArray[lastIndex-1] = priceIndexArray[lastIndex]
                        priceIndexArray[lastIndex] = temp
                        lastIndex -= 1

            responseText = f"我們有賣的最低價的產品如下:\n"
            responseText = responseText + "1" + "." + dataSet["ProductName"][priceIndexArray[0][1]] + " (" + dataSet["RAM"][priceIndexArray[0][1]] + " " + "RAM"
            if dataSet["ROM"][priceIndexArray[0][1]] != "No":
                responseText = responseText + "/" + dataSet["ROM"][priceIndexArray[0][1]] + " ROM"
            if dataSet["SSD"][priceIndexArray[0][1]] != "No":
                responseText = responseText + "/" + dataSet["SSD"][priceIndexArray[0][1]] + " SSD"
            responseText = responseText + ") "
            try:
                math.isnan(dataSet["Processor"][priceIndexArray[0][1]])
            except:
                responseText = responseText + dataSet["Processor"][priceIndexArray[0][1]] + " "
            try:
                math.isnan(dataSet["GPU"][priceIndexArray[0][1]])
            except:
                responseText = responseText + dataSet["GPU"][priceIndexArray[0][1]] + " "
            responseText = responseText + " " + dataSet["Color"][priceIndexArray[0][1]] + " " + dataSet["Price"][priceIndexArray[0][1]] + "\n"

            i = 1
            while priceIndexArray[0][1] == priceIndexArray[i][1]:
                responseText = responseText + str(i+1) + "." + dataSet["ProductName"][priceIndexArray[i][1]] + " (" + \
                               dataSet["RAM"][priceIndexArray[i][1]] + " " + "RAM"
                if dataSet["ROM"][priceIndexArray[i][1]] != "No":
                    responseText = responseText + "/" + dataSet["ROM"][priceIndexArray[i][1]] + " ROM"
                if dataSet["SSD"][priceIndexArray[i][1]] != "No":
                    responseText = responseText + "/" + dataSet["SSD"][priceIndexArray[i][1]] + " SSD"
                try:
                    math.isnan(dataSet["Processor"][priceIndexArray[i][1]])
                except:
                    responseText = responseText + dataSet["Processor"][priceIndexArray[i][1]] + " "
                try:
                    math.isnan(dataSet["GPU"][priceIndexArray[i][1]])
                except:
                    responseText = responseText + dataSet["GPU"][priceIndexArray[i][1]] + " "
                responseText = responseText + dataSet["Color"][priceIndexArray[i][1]] + dataSet["Price"][
                    priceIndexArray[i][1]] + "\n"
                i += 1
        else:
            responseText = f"我們沒有賣此類的產品"
    elif intent == "AskSpecificCompany&ProductSold":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        Brands, productBrandsCol, ProductTpye, ProductTpyeCol = extractingJsonReqProductTypeAndBrands(req, dataSet, isFallback=False)
        j = 1
        print(productBrandsCol)
        responseText = f"我們有賣的的{Brands}{ProductTpye}產品如下:\n"
        index = []
        for i in range(len(dataSet)):
            try:
                math.isnan(dataSet["產品公司"][i])
            except:
                index.append(i)
        if len(index) > 0:
            for i in index:
                if productBrandsCol[i] == Brands and ProductTpyeCol[i] == ProductTpye:
                    responseText = responseText + str(j) + "." + dataSet["ProductName"][i] + " (" + \
                                       dataSet["RAM"][i]
                    print(dataSet["產品公司"][i])
                    if dataSet["ROM"][i] != "No":
                        responseText = responseText + "/" + str(dataSet["ROM"][i]) + " ROM"
                    if dataSet["SSD"][i] != "No":
                        responseText = responseText + "/" + dataSet["SSD"][i] + " SSD"
                    try:
                        math.isnan(dataSet["Processor"][i])
                    except:
                        responseText = responseText + " " + dataSet["Processor"][i] + " "
                    try:
                        math.isnan(dataSet["GPU"][i])
                    except:
                        responseText = responseText + dataSet["GPU"][i] + " "
                    responseText = responseText + dataSet["Color"][i] + "\n"
                    j += 1
        else:
            responseText = "我們沒有賣這類的產品"
        if len(responseText) > 5000:
            responseText = "字數太多沒有辦法印出請問您是否能給產品名稱？"
    elif intent == "WhatBrandsOrProductTypeDoWeSold(P)":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)

        Brands, productBrandsCol, ProductTpye, ProductTpyeCol = extractingJsonReqProductTypeAndBrands(req, dataSet, isFallback=False)
        index = []
        j = 1
        for i in range(len(dataSet)):
            try:
                math.isnan(dataSet["產品公司"][i])
            except:
                index.append(i)
        if len(index) > 0:
            for i in index:
                if productBrandsCol[i] == Brands and ProductTpyeCol[i] == ProductTpye:
                    responseText = responseText + str(j) + "." + dataSet["ProductName"][i] + " (" + \
                                   dataSet["RAM"][i]
                    if dataSet["ROM"][i] != "No":
                        responseText = responseText + "/" + str(dataSet["ROM"][i]) + " ROM"
                    if dataSet["SSD"][i] != "No":
                        responseText = responseText + "/" + dataSet["SSD"][i] + " SSD"
                    try:
                        math.isnan(dataSet["Processor"][i])
                    except:
                        responseText = responseText + dataSet["Processor"][i] + " "
                    try:
                        math.isnan(dataSet["GPU"][i])
                    except:
                        responseText = responseText + dataSet["GPU"][i] + " "
                    responseText = responseText + " " + dataSet["Color"][i] + " " + dataSet["Price"][i] + "\n"
                    j += 1
        else:
            responseText = f"我們沒有賣此類的產品"
        if len(responseText) > 5000:
            responseText = "字數太多沒有辦法印出請問您是否能給產品名稱？"
    elif intent == "WhatBrandsOrProductTypeDoWeSold(P) - custom":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        ProductName = req["queryResult"]["parameters"]["phone_productname"]
        j = 1
        for i in range(len(dataSet)):
            try:
                math.isnan(dataSet["產品公司"][i])
            except:
                if dataSet["ProductName"][i] == ProductName:
                    responseText = responseText + str(j) + "." + dataSet["ProductName"][i] + " (" + dataSet["RAM"][i]
                    if dataSet["ROM"][i] != "No":
                        responseText = responseText + "/" + str(dataSet["ROM"][i]) + " ROM"
                    if dataSet["SSD"][i] != "No":
                        responseText = responseText + "/" + dataSet["SSD"][i] + " SSD"
                    try:
                        math.isnan(dataSet["Processor"][i])
                    except:
                        responseText = responseText + dataSet["Processor"][i] + " "
                    try:
                        math.isnan(dataSet["GPU"][i])
                    except:
                        responseText = responseText + dataSet["GPU"][i] + " "
                    responseText = responseText + " " + dataSet["Color"][i] + " " + dataSet["Price"][i] + "\n"
                    responseText = f"""{responseText}:
售價:{dataSet["Price"][i]}\n
"""
                    j += 1
    elif intent == "RecommendByProductTypeOrBrands":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        Brands, productBrandsCol, ProductTpye, ProductTpyeCol = extractingJsonReqProductTypeAndBrands(req, dataSet, isFallback=False)
        matchIndex = []
        for i in range(len(ProductTpyeCol)):
            if Brands == productBrandsCol[i] and productBrandsCol[i] == Brands:
                matchIndex.append(i)
        RecommendIndex = matchIndex[random.randint(0, len(matchIndex)-1)]
        print(RecommendIndex)
        productName = dataSet["ProductName"][RecommendIndex]
        ROM = dataSet["ROM"][RecommendIndex]
        SSD = dataSet["SSD"][RecommendIndex]
        RAM = dataSet["RAM"][RecommendIndex]
        links = dataSet["Links"][RecommendIndex]
        responseText = f"""我們推薦：
{productName} {RAM}RAM {ROM}ROM {SSD}SSD
{links}
"""
    elif intent == "WhatBrandsOrProductTypeDoWeSold(P) - fallback":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        Brands, productBrandsCol, ProductTpye, ProductTpyeCol = extractingJsonReqProductTypeAndBrands(req, dataSet,
                                                                                                      isFallback=True)
        j = 1
        responseText = f"以下我們有的一部分{Brands}{ProductTpye}的產品："
        for i in range(len(dataSet)):
            try:
                math.isnan(dataSet["產品公司"][i])
            except:
                if productBrandsCol[i] == Brands and ProductTpyeCol[i] == ProductTpye:
                    JoinString = str(j) + "." + dataSet["ProductName"][i] + " (" + \
                                   dataSet["RAM"][i]
                    if dataSet["ROM"][i] != "No":
                        JoinString = JoinString + "/" + str(dataSet["ROM"][i]) + " ROM"
                    if dataSet["SSD"][i] != "No":
                        JoinString = JoinString + "/" + dataSet["SSD"][i] + " SSD"
                    try:
                        math.isnan(dataSet["Processor"][i])
                    except:
                        JoinString = JoinString + dataSet["Processor"][i] + " "
                    try:
                        math.isnan(dataSet["GPU"][i])
                    except:
                        JoinString = JoinString + dataSet["GPU"][i] + " "
                    JoinString = JoinString + " " + dataSet["Color"][i] + " " + dataSet["Price"][i]
                    if len(JoinString) + len(responseText) >= 500:
                        break
                    responseText = responseText + "\n" + JoinString
                    j += 1
    elif intent == "AskSpecificCompany&ProductSold - fallback":
        myProductDataSetDir = "myProductDataSets.xlsx"
        dataSet = pd.read_excel(myProductDataSetDir)
        Brands, productBrandsCol, ProductTpye, ProductTpyeCol = extractingJsonReqProductTypeAndBrands(req, dataSet,
                                                                                                      isFallback=True)
        j = 1
        responseText = f"以下我們有的一部分{Brands}{ProductTpye}的產品："
        for i in range(len(dataSet)):
            try:
                math.isnan(dataSet["產品公司"][i])
            except:
                if productBrandsCol[i] == Brands and ProductTpyeCol[i] == ProductTpye:
                    JoinString = str(j) + "." + dataSet["ProductName"][i] + " (" + \
                                 dataSet["RAM"][i]
                    if dataSet["ROM"][i] != "No":
                        JoinString = JoinString + "/" + str(dataSet["ROM"][i]) + " ROM"
                    if dataSet["SSD"][i] != "No":
                        JoinString = JoinString + "/" + dataSet["SSD"][i] + " SSD"
                    try:
                        math.isnan(dataSet["Processor"][i])
                    except:
                        JoinString = JoinString + dataSet["Processor"][i] + " "
                    try:
                        math.isnan(dataSet["GPU"][i])
                    except:
                        JoinString = JoinString + dataSet["GPU"][i] + " "
                    JoinString = JoinString + " " + dataSet["Color"][i]
                    if len(JoinString) + len(responseText) >= 500:
                        break
                    responseText = responseText + "\n" + JoinString
                    j += 1
    elif intent == "helpOpenApplication":
        responseText = ""
    elif intent == "Default Fallback Intent":
        urlForAskedQuestion = "https://www.google.com/search?q=" + req["queryResult"]["queryText"]
        page = requests.get(urlForAskedQuestion)
        soup = BeautifulSoup(page.text, "lxml")
        htmlSpanClassAttr = soup.find("span", {"class": "xUrNXd UMOHqf"})
        if htmlSpanClassAttr.text == "找不到符合搜尋字詞「":
            crashText = req["lol"]
        else:
            responseText = urlForAskedQuestion
    else:
        responseText = f"There are no fulfillment responses defined for Intent {intent}"

    # You can also use the google.cloud.dialogflowcx_v3.types.WebhookRequest protos instead of manually writing the
    # json object

    res = {
            "fulfillmentMessages": [
                {
                    "text": {
                        "text": [
                            responseText
                        ]
                    }
                }
            ]
            }

    return res


@app.route('/test', methods=['GET', 'POST'])
def in_test_page():
    res = {
            "fulfillmentMessages": [
                {
                    "text": {
                        "text": [
                            "Text response from webhook"
                        ]
                    }
                }
            ]
            }
    return res


def translateFunc(dir, countryName):
    dataSet = pd.read_excel(dir)
    CH_col = dataSet["CountryCH"]
    for i in range(len(CH_col)):
        if CH_col[i] == countryName:
            translatedName = dataSet["CountryEN"]
            return translatedName[i]
    return ""


def extractingJsonReqProductTypeAndBrands(req, dataSet, isFallback):
    if not isFallback:
        if req["queryResult"]["parameters"]["Brands"] != "":
            Brands = req["queryResult"]["parameters"]["Brands"]
            productBrandsCol = dataSet["產品公司"]
        else:
            Brands = True
            productBrandsCol = []
            for i in range(len(dataSet)):
                productBrandsCol.append(True)

        if req["queryResult"]["parameters"]["SaleProducttype"] != "":
            ProductTpye = req["queryResult"]["parameters"]["SaleProducttype"]
            ProductTpyeCol = dataSet["機種"]
        else:
            ProductTpye = True
            ProductTpyeCol = []
            for i in range(len(dataSet)):
                ProductTpyeCol.append(True)
        return Brands, productBrandsCol, ProductTpye, ProductTpyeCol
    else:
        if req["queryResult"]["outputContexts"][0]['parameters']["Brands"] != "":
            Brands = req["queryResult"]["outputContexts"][0]['parameters']["Brands"]
            productBrandsCol = dataSet["產品公司"]
        else:
            Brands = True
            productBrandsCol = []
            for i in range(len(dataSet)):
                productBrandsCol.append(True)
        if req["queryResult"]["outputContexts"][0]['parameters']["SaleProducttype"] != "":
            ProductTpye = req["queryResult"]["outputContexts"][0]['parameters']["SaleProducttype"]
            ProductTpyeCol = dataSet["機種"]
        else:
            ProductTpye = True
            ProductTpyeCol = []
            for i in range(len(dataSet)):
                ProductTpyeCol.append(True)
        return Brands, productBrandsCol, ProductTpye, ProductTpyeCol

def findProductOnlyName(dir, productName):
    dataSet = pd.read_excel(dir)
    productNameCol = dataSet["ProductName"]
    for i in range(len(productNameCol)):
        if productNameCol[i] == productName:
            return i
    return "None"


def findProductWithValue(dir, productName, value, lengthOfVolume, ProductColor, CPU, GPU, unitType):
    dataSet = pd.read_excel(dir)
    productNameCol = dataSet["ProductName"]
    romCol = dataSet["ROM"]
    ramCol = dataSet["RAM"]
    ssdCol = dataSet["SSD"]

    if ProductColor == True:
        productColorCol = []
        for i in range(len(productNameCol)):
            productColorCol.append(True)
    else:
        productColorCol = dataSet["Color"]
    if CPU == True:
        cpuCol = []
        for i in range(len(productNameCol)):
            cpuCol.append(True)
    else:
        cpuCol = dataSet["Processor"]
    if GPU == True:
        gpuCol = []
        for i in range(len(productNameCol)):
            gpuCol.append(True)
    else:
        gpuCol = dataSet["GPU"]

    if lengthOfVolume == 3:
        for i in range(len(productNameCol)):
            if productNameCol[i] == productName and romCol[i] == (str(value[2]) + unitType[2]) and ssdCol[i] == (str(value[1]) + unitType[1]) and ramCol[i] == (str(value[0]) + unitType[0]) and cpuCol[i] == CPU and gpuCol[i] == GPU and productColorCol[i] == ProductColor:
                return i
    elif lengthOfVolume == 2:
        for i in range(len(productNameCol)):
            if value[0] <= 64: 
                if (productNameCol[i] == productName and (romCol[i] == (str(value[1]) + unitType[1]) or ssdCol[i] == (str(value[1]) + unitType[1])) and ramCol[i] == (str(value[0]) + unitType[0]) and cpuCol[i] == CPU and gpuCol[i] == GPU and productColorCol[i] == ProductColor):
                    return i
            else:
                if productNameCol[i] == productName and romCol[i] == (str(value[1]) + unitType[1]) and ssdCol[i] == (str(value[0]) + unitType[0]) and cpuCol[i] == CPU and gpuCol[i] == GPU and productColorCol[i] == ProductColor:
                    return i
    else:
        for i in range(len(productNameCol)):
            if value[0] <= 64 and unitType[0] != "TB":
                if productNameCol[i] == productName and ramCol[i] == (str(value[0]) + unitType[0]) and cpuCol[i] == CPU and gpuCol[i] == GPU and productColorCol[i] == ProductColor:
                    return i
            else:
                if (productNameCol[i] == productName and (romCol[i] == (str(value[0]) + unitType[0]) or ssdCol[i] == (str(value[0]) + unitType[0])) and cpuCol[i] == CPU and gpuCol[i] == GPU and productColorCol[i] == ProductColor):
                    return i
    return "None"


def intentPhoneProduct(req):
    productName = req["queryResult"]["parameters"]["phone_productname"]
    unitVolume = [0, 0, 0]
    unitType = ["GB", "GB", "GB"]
    ErrorMassage = ""
    if req["queryResult"]["parameters"]["color"] != "":
        ProductColor = req["queryResult"]["parameters"]["color"]
    elif req["queryResult"]["parameters"]["ProductColor"] != "":
        ProductColor = req["queryResult"]["parameters"]["ProductColor"] + "色"
    else:
        ProductColor = True
    if req["queryResult"]["parameters"]["CPUentities"] != "":
        CPU = req["queryResult"]["parameters"]["CPUentities"]
    else:
        CPU = True
    if req["queryResult"]["parameters"]["GPUentities"] != "":
        GPU = req["queryResult"]["parameters"]["GPUentities"]
    else:
        GPU = True
    myProductDataSetDir = "myProductDataSets.xlsx"
    dataSet = pd.read_excel(myProductDataSetDir)
    isNotMatchRequirement = False
    try:
        lenghtOfVolumeElement = len(req["queryResult"]["parameters"]["unit-information"])

        if lenghtOfVolumeElement == 0:
            crashProgram = req["random"]
        elif lenghtOfVolumeElement >= 2:
            for i in range(lenghtOfVolumeElement):
                if req["queryResult"]["parameters"]["unit-information"][i]["unit"] == "TB":
                    unitVolume[i] = int(req["queryResult"]["parameters"]["unit-information"][i]["amount"]) * 1000
                else:
                    unitVolume[i] = int(req["queryResult"]["parameters"]["unit-information"][i]["amount"])
            for i in range(lenghtOfVolumeElement):
                if i != 0 and unitVolume[i] < unitVolume[i-1]:
                    temp = unitVolume[i-1]
                    unitVolume[i-1] = unitVolume[i]
                    unitVolume[i] = temp
            if unitVolume[1] >= 1000:
                unitVolume[1] = int(unitVolume[1] / 1000)
                unitType[1] = "TB"
            if unitVolume[2] >= 1000:
                unitVolume[2] = int(unitVolume[2] / 1000)
                unitType[2] = "TB"
            index = findProductWithValue(myProductDataSetDir, productName, unitVolume, lenghtOfVolumeElement, ProductColor, CPU, GPU, unitType)
        else:
            lenghtOfVolumeElement = 1
            unitVolume = int(req["queryResult"]["parameters"]["unit-information"][0]["amount"])
            unitType[0] = req["queryResult"]["parameters"]["unit-information"][0]["unit"]
            index = findProductWithValue(myProductDataSetDir, productName, unitVolume, lenghtOfVolumeElement, ProductColor, CPU, GPU, unitType)
    except:
        try:
            lenghtOfVolumeElement = len(req["queryResult"]["parameters"]["NumberForIntentPhoneProduct"])
            if lenghtOfVolumeElement == 0:
                lenghtOfVolumeElement = 0
                index = findProductOnlyName(myProductDataSetDir, productName)
            else:
                for i in range(lenghtOfVolumeElement):
                    unitVolume[i] = int(req["queryResult"]["parameters"]["NumberForIntentPhoneProduct"][i])
                index = "None"
                isNotMatchRequirement = True
                data = [index, ErrorMassage]
                return data
        except:
            lenghtOfVolumeElement = 0
            checkVolume = 0
            index = findProductOnlyName(myProductDataSetDir, productName)

    if index == "None":
        if isNotMatchRequirement:
            ErrorMassage = "Value:"
            for i in range(lenghtOfVolumeElement):
                ErrorMassage = ErrorMassage + f" {unitVolume[i]}"
            ErrorMassage = ErrorMassage + "meet our requirement please input GB and TB"
        else:
            if lenghtOfVolumeElement >= 3:
                ErrorMassage = f"No Such Product {productName}, {unitVolume[1]}{unitType[1]} SSD, {unitVolume[2]}{unitType[2]} ROM, {unitVolume[0]}{unitType[0]} RAM"
            elif lenghtOfVolumeElement >= 2:
                if unitVolume[0] <= 64:
                    ErrorMassage = f"No Such Product {productName}, {unitVolume[1]}{unitType[1]} ROM|| SSD, {unitVolume[0]}{unitType[0]} RAM"
                else:
                    ErrorMassage = f"No Such Product {productName}, {unitVolume[1]}{unitType[1]} ROM, {unitVolume[0]}{unitType[0]} SSD"
            elif lenghtOfVolumeElement == 1:
                if unitVolume[0] <= 64:
                    ErrorMassage = f"No Such Product {productName}, {unitVolume[0]}{unitType[0]} RAM"
                else:
                    ErrorMassage = f"No Such Product {productName}, {unitVolume[0]}{unitType[0]} SSD||ROM"
            else:
                ErrorMassage = f"No Such Product {productName}"
    data = [index, ErrorMassage]
    return data


if __name__ == "__main__":
    app.run()