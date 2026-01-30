import React, { Dispatch, SetStateAction, useCallback, useState } from 'react'
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
  setExpandAddressBook?: Dispatch<SetStateAction<boolean>>;
  lastMessageId?: number;
}

function ShowPerson(
  { from, r, handleClickPerson, setExpandAddressBook, lastMessageId }: Props
) {
  const [expand, setExpand] = useState<boolean>(false);
  const { handleRequest } = useAddFriend();

  // Prevent unneccesary rerender
  const PersonClickHandler = useCallback(() => {
    // Show from chat
    if (handleClickPerson !== undefined && setExpandAddressBook !== undefined && lastMessageId !== undefined) {
      handleClickPerson({
        OthersId: r.userId, LastMessageId: lastMessageId
      }, r.id)
      setExpandAddressBook(false);
    } else {
      setExpand(!expand)
    }
  }, [handleClickPerson, setExpandAddressBook, lastMessageId, r.userId, r.id, expand])

  return (
    <div className='ShowPersonContainer'>
      <div className='flex EachPersonContainer asButton'
        role='button'
        onClick={() => PersonClickHandler()}
      >
        <div className='UserIconOuter'>
          <img
            className='UserIcon'
            src={wheat}
            alt="profilePic"
          />
        </div>
        <div className="PersonInfoText"><p>Id:{r.id}</p><p>Name:{r.name}</p></div>
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