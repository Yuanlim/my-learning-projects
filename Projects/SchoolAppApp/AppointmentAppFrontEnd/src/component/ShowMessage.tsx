import React from 'react'
import { toDate } from '../Community';
import { ChatMessage } from '../types/Chat';

type Props = {
  chatMessage: ChatMessage
}

function ShowMessage({ chatMessage }: Props) {
  const ChatBoxAlignHandler = (ISended: boolean): React.CSSProperties => {
    console.log(ISended);
    let leftOrRight: "left" | "right" = "right";
    if (ISended) leftOrRight = "left";
    return {
      justifyItems: leftOrRight,
      padding: "20px"
    }
  }

  return (
    <div style={ChatBoxAlignHandler(chatMessage.iSended)}>
      <div className="card ChatBox">
        {chatMessage.content}
      </div>
      <p>{toDate(chatMessage.postDateTime)}</p>
    </div>
  )
}

export default ShowMessage