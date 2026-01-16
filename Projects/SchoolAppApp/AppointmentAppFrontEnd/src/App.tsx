import React from "react";

// Navigation
import { Routes, Route } from "react-router-dom";

// Redux 

// Used pages
import LoR from "./LoR";
import Header from "./Header";
import Nothingness from "./Nothingness";
import Footer from "./Footer";
import Community from "./Community";
import Chat from "./Chat";
import Shopping from "./Shopping";
import StudyMat from "./StudyMat";
import Self from "./Self";
import { LoRDataProvider } from "./context/LoRContext";
import Friend from "./Friend";
import AddFriend from "./AddFriend";
import Pending from "./Pending";
import Block from "./Block";
import Accepted from "./Accepted";
import { PendingDataProvider } from "./context/PendingContext";
import { BlockDataProvider } from "./context/BlockContext";
import { AcceptedDataProvider } from "./context/AcceptedContext";
import ProductPage from "./ProductPage";

export const API_URL = "http://localhost:5095/";

function App() {
  // const loginState: UserDataType = useAppSelector((state) => state.login);

  return (
    <div className="App">
      <Header />
      <Routes>
        <Route path="/Login" element={<LoRDataProvider><LoR /></LoRDataProvider>} />
        <Route path="/" element={<Nothingness />} />
        <Route path="/Community" element={<Community />} />
        <Route path="/Chat" element={<AcceptedDataProvider><Chat /></AcceptedDataProvider>} />
        <Route path="/Shopping" element={<Shopping />} />
        <Route path="/Friend" element={<PendingDataProvider><Friend /></PendingDataProvider>} />
        <Route path="/StudyMat" element={<StudyMat />} />
        <Route path="/Self" element={<Self />} />
        <Route path="/AddFriend" element={<AddFriend />}></Route>
        <Route path="/Pending" element={<PendingDataProvider><Pending /></PendingDataProvider>}></Route>
        <Route path="/Block" element={<BlockDataProvider><Block /></BlockDataProvider>}></Route>
        <Route path="/Accepted" element={
          <AcceptedDataProvider><Accepted /></AcceptedDataProvider>
        }>
        </Route>
        <Route path="/ProductPage/:id" element={<ProductPage />}></Route>
      </Routes>
      <Footer />
    </div>
  );
}

export default App;
