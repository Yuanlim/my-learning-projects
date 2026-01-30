import React from 'react'
import { GoCheck, GoX } from 'react-icons/go'
import { BlockRequestPayload, PersonDataType, StatusRequestPayload } from '../types/Friend'
import { useAppDispatch } from '../hooks/useReduxHook'
import { moveUserFromList, removeUserFromList } from '../redux/relation'

type Props = {
  r: PersonDataType
  handleRequest: (payload: StatusRequestPayload | BlockRequestPayload) => Promise<boolean>
}

const PendingButton = ({ r, handleRequest }: Props) => {
  const dispatch = useAppDispatch();

  return (
    <>
      <GoCheck role="button"
        style={{ color: "green", cursor: 'pointer', height: "50px", width: "50px", padding: "5px" }}
        onClick={async () => {
          const success: boolean = await handleRequest({ ToUserId: r.userId, Frps: "Accepted" });
          if (success) {
            dispatch(moveUserFromList({ AskFor: "Pending", To: "Accepted", UserId: r.userId }))
          }
        }}
      />
      <GoX role="button"
        style={{ color: "red", cursor: 'pointer', height: "50px", width: "50px", padding: "5px" }}
        onClick={async () => {
          const success: boolean = await handleRequest({ ToUserId: r.userId, Frps: "Denied" });
          if (success) dispatch(removeUserFromList({ AskFor: "Pending", UserId: r.userId }))
        }
        }
      />
    </>
  )
}

export default PendingButton