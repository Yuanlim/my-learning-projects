import React from 'react'
import { BsFillSendPlusFill } from "react-icons/bs";
import { BsFillSendSlashFill } from "react-icons/bs";

import wheat from "../img/wheat.png"
import { BlockRequestPayload, PersonDataType, StatusRequestPayload } from '../types/Friend';

type Props = {
  personInfo: PersonDataType | null,
  handleRequest: (payload: StatusRequestPayload | BlockRequestPayload) => Promise<boolean>;
}

const SearchPerson = ({ personInfo, handleRequest }: Props) => {
  return (
    <div className='card contextCenter'
      style={{ flexGrow: "1" }}
    >

      {personInfo ?
        <div className='flex contextCenter'
          style={{ flexDirection: "column", gap: "40px", justifyContent: 'space-around' }}
        >
          <div
            style={{ backgroundColor: "whitesmoke", borderRadius: "50%", padding: "20px" }}
          >
            <img
              style={{ width: "80px", height: "auto" }}
              src={wheat}
              alt="profilePic"
            />
          </div>
          <h2>{personInfo.id}</h2>
          <h3>{personInfo.name}</h3>
          <div className="flex" style={{ justifyContent: 'space-around', width: "100%" }}>
            <BsFillSendPlusFill role='button'
              style={{ cursor: "pointer", width: "30px", height: "auto" }}
              onClick={() => handleRequest(
                { ToUserId: personInfo.userId, Frps: "Pending" }
              )}
            />
            <BsFillSendSlashFill
              role='button' style={{ cursor: "pointer", width: "30px", height: "auto" }}
              onClick={() => handleRequest({
                ToUserId: personInfo.userId
              })}
            />
          </div>
        </div>
        : "No results"}
    </div>
  )
}

export default SearchPerson