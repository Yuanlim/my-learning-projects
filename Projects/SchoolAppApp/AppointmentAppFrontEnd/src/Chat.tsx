import React, { useEffect, useRef, useState } from 'react'
import useCheckDirectAccessor from './hooks/useCheckDirectAccessor';
import { useAcceptedContext } from './hooks/useContext';
import { useAppDispatch, useAppSelector } from './hooks/useReduxHook';
import ShowPerson from './component/ShowPerson';
import { fetchData } from './API/FetchAPI';
import { setPrompText } from './redux/generic';
import ShowMessage from './component/ShowMessage';
import { FiSend } from "react-icons/fi";
import { ChatMessage, GetChatPayload, PostChatPayload, ResponseChatMessage } from './types/Chat';
import { TfiAlignJustify } from 'react-icons/tfi';
import useWindowSize from './hooks/useWindowSize';
import "../src/chat.css";

const Chat = () => {
  useCheckDirectAccessor();
  const { handleReFetchAccepted } = useAcceptedContext();
  const [chatMessages, setChatMessages] = useState<ChatMessage[]>([]);
  const [lastMessageId, setLastMessageId] = useState<number>(0);
  const [currentReadId, setCurrentReadId] = useState<string | undefined>(undefined);
  const [currentReadUserId, setCurrentReadUserId] = useState<number | undefined>(undefined);
  const [expandAddressBook, setExpandAddressBook] = useState<boolean>(false);
  const { width } = useWindowSize();
  const lastMessageIdRef = useRef<number>(0);
  const currentReadIdRef = useRef<string | undefined>(undefined);
  const currentReadUserIdRef = useRef<number | undefined>(undefined);
  const [sendText, setSendText] = useState<string>("");
  const isFetchingRef = useRef<boolean>(false);
  const acceptedState = useAppSelector((state) => state.relation["Accepted"])
  const dispatch = useAppDispatch();

  useEffect(() => {
    handleReFetchAccepted();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  useEffect(() => { lastMessageIdRef.current = lastMessageId; }, [lastMessageId]);
  useEffect(() => { currentReadIdRef.current = currentReadId; }, [currentReadId]);
  useEffect(() => { currentReadUserIdRef.current = currentReadUserId; }, [currentReadUserId]);

  // Refetch for new message every 2s
  useEffect(() => {
    const id = setInterval(async () => {
      const OthersId = currentReadIdRef.current;
      const OthersUserId = currentReadUserIdRef.current;
      if (!OthersId || !OthersUserId) return; // No address selection yet
      if (isFetchingRef.current) return; // Busy

      isFetchingRef.current = true;
      await handleClickPerson({
        OthersId: OthersUserId,
        LastMessageId: lastMessageIdRef.current
      }, OthersId)
      isFetchingRef.current = false;
    }, 2000);

    return () => clearInterval(id);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleClickPerson = async (payload: GetChatPayload, stringId: string) => {
    const URL = `Chat/Get`
    const { data, success } = await fetchData<GetChatPayload, ResponseChatMessage>(
      { URL: URL, method: "post", credentials: true, payload: payload }
    );
    if (!success) dispatch(setPrompText(data as string));
    else {
      const newChat = (data as ResponseChatMessage).chatMessages ?? [];
      if (newChat.length > 0) // ok but nothing passed, preserved
      {
        setChatMessages(previousChat => [...previousChat, ...newChat]);
        setLastMessageId((data as ResponseChatMessage).lastMessageId)
      }
      setCurrentReadId(stringId);
      setCurrentReadUserId(payload.OthersId);
    }
  }

  const handleClickSend = async (payload: PostChatPayload) => {
    if (!payload.ToUserId)
      return dispatch(setPrompText("Please select someone in your address book to send the message"));
    if (!payload.Content) return dispatch(setPrompText("Content cant be empty!"));

    const URL = `Chat/Post`
    const { data, success } = await fetchData<PostChatPayload, ResponseChatMessage>(
      { URL: URL, method: "post", credentials: true, payload: payload }
    );

    if (!success) dispatch(setPrompText(data as string));
  }

  return (
    <main className='main card' style={{ width: "100%", height: "auto" }}>
      {/* Address book */}
      <div className='ChatAddressBook'
        style={{ display: width > 500 || expandAddressBook ? "block" : "none" }}
      >
        {acceptedState.length > 0 && acceptedState.map(p =>
          <ShowPerson from='Chat' r={p} handleClickPerson={handleClickPerson} key={p.userId} />)
        }
        {acceptedState.length === 0 && "Add a friend to appear here."}
      </div>
      {/* Main content */}
      <div className="flex MessageDisplayer">
        {/* Show selected person id */}
        <div className='MessageDisplayerTop'>
          <TfiAlignJustify
            className='ExpandAddressBook asButton'
            style={{ display: width < 500 ? "block" : "none" }}
            onClick={() => setExpandAddressBook(!expandAddressBook)}
          />
          <p>{currentReadId}</p>
        </div>
        {/* chat room */}
        <section
          className="flex"
          style={{
            flexGrow: "1", flexDirection: "column", overflowY: "auto"
          }}
        >
          {/* chat boxes */}
          {!expandAddressBook && Array.isArray(chatMessages) &&
            chatMessages.map(m =>
              <ShowMessage key={"Chat" + m.postDateTime} chatMessage={m} />
            )}
        </section>
        {/* chat input && send */}
        <div className="flex ChatInputAndSendContainer">
          <input
            className='ChatInputBox'
            type="text" name="userChatInput" id="userChatInput"
            value={sendText}
            onChange={(e) => setSendText(e.target.value)}
          />
          <button
            type='button'
            className='withTip ChatSendButton'
            onClick={() => {
              handleClickSend({ ToUserId: currentReadUserId, Content: sendText })
            }}
          >
            <FiSend className='chatSendIcon' />
            <span
              className='tooltip up'
              style={{ left: "-55%" }} // offset
            >
              send
            </span>
          </button>
        </div>
      </div>
    </main>
  )
}

export default Chat