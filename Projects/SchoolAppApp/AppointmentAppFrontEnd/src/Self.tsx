import React from 'react'
import useCheckDirectAccessor from './hooks/useCheckDirectAccessor';
import { setPrompText } from './redux/generic';
import { useNavigate } from 'react-router-dom';
import { fetchData } from './API/FetchAPI';

type Props = {}

const Self = (props: Props) => {
  useCheckDirectAccessor();
  const navigate = useNavigate();

  const handleLogOut = async (): Promise<void> => {
    const URL = `logout`;
    const { success } = await fetchData<undefined, void>(
      { URL: URL, method: "post", credentials: true, payload: undefined }
    );
    if (success) {
      setPrompText("Logged Out");
      navigate("/Login");
    }
  }

  return (
    <main className='main' style={{ width: "100%", flexFlow: "column", border: "2px solid #444" }}>
      <div role="button" className='card' style={{ justifyContent: 'center', alignItems: "center", height: "80px", cursor: "pointer" }} onClick={handleLogOut}>
        LogOut
      </div>
    </main>
  )
}

export default Self