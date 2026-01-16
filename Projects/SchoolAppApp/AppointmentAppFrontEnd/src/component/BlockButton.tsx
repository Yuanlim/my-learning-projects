import React from 'react'
import { CgUnblock } from "react-icons/cg";
import { BlockRequestPayload, PersonDataType, StatusRequestPayload } from '../types/Friend'
import { removeUserFromList } from '../redux/relation';
import { useAppDispatch } from '../hooks/useReduxHook';

type Props = {
  r: PersonDataType
  handleRequest: (payload: StatusRequestPayload | BlockRequestPayload) => Promise<boolean>
}

const BlockButton = ({ r, handleRequest }: Props) => {
  const dispatch = useAppDispatch();

  return (
    <>
      <CgUnblock role="button"
        style={{ color: "green", cursor: 'pointer', height: "50px", width: "50px", padding: "5px" }}
        onClick={async () => {
          const success: boolean = await handleRequest({ ToUserId: r.userId, Frps: "Denied" });
          if (success) dispatch(removeUserFromList({ AskFor: "Block", UserId: r.userId }))
        }}
      />
    </>
  )
}

export default BlockButton