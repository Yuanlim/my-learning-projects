import React from 'react'
import { AiOutlineUserDelete } from "react-icons/ai";
import { BlockRequestPayload, PersonDataType, StatusRequestPayload } from '../types/Friend'

type Props = {
  r: PersonDataType
  handleRequest: (payload: StatusRequestPayload | BlockRequestPayload) => Promise<boolean>
}

const AcceptButton = ({ r, handleRequest }: Props) => {
  return (
    <>
      <AiOutlineUserDelete
        className='Accept__Button'
        role="button"
        onClick={() => handleRequest(
          { ToUserId: r.userId, Frps: "Denied" }
        )}
      />
    </>
  )
}

export default AcceptButton