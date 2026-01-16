import React, { useState } from 'react'
import { PersonDataType } from '../types/Friend'
import wheat from "../img/wheat.png"
import { RelationType } from '../redux/relation'
import useAddFriend from '../hooks/useAddFriend'
import PendingButton from './PendingButton'
import BlockButton from './BlockButton'
import AcceptButton from './AcceptButton'
import { GetChatPayload } from '../types/Chat'

type Props = {
  from: RelationType | "Chat",
  r: PersonDataType,
  handleClickPerson?: (payload: GetChatPayload, stringId: string) => void;
}

function ShowPerson({ from, r, handleClickPerson }: Props) {
  const [expand, setExpand] = useState<boolean>(false);
  const { handleRequest } = useAddFriend();

  return (
    <div style={{ margin: "20px", border: "2px solid white", height: "auto" }}>
      <div className='flex'
        style={{
          padding: "10px",
          gap: "40px",
          cursor: 'pointer'
        }}
        role='button'
        onClick={() =>
          handleClickPerson !== undefined ? handleClickPerson({
            OthersId: r.userId, LastMessageId: 0
          }, r.id) : setExpand(!expand)
        }
      >
        <div
          style={{ backgroundColor: "whitesmoke", borderRadius: "50%", padding: "15px" }}
        >
          <img
            style={{ width: "50px", height: "auto" }}
            src={wheat}
            alt="profilePic"
          />
        </div>
        <div style={{ alignContent: "center" }}><p>Id:{r.id}</p><p>Name:{r.name}</p></div>
      </div>
      {from !== "Chat" && <div style={{
        height: "50px",
        display: expand ? "flex" : "none",
        backgroundColor: "white",
        justifyContent: "space-around"
      }}>
        {from === "Pending" && <PendingButton r={r} handleRequest={handleRequest} />}
        {from === "Block" && <BlockButton r={r} handleRequest={handleRequest} />}
        {from === "Accepted" && <AcceptButton r={r} handleRequest={handleRequest} />}
      </div>}
    </div>
  )
}

export default ShowPerson