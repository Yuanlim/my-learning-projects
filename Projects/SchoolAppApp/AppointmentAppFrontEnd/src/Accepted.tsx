import React, { useEffect } from 'react'
import ShowPerson from './component/ShowPerson'
import { useAppSelector } from './hooks/useReduxHook';
import useCheckDirectAccessor from './hooks/useCheckDirectAccessor';
import { useAcceptedContext } from './hooks/useContext';

function Accepted() {
  useCheckDirectAccessor();
  const { handleReFetchAccepted, isLoading } = useAcceptedContext();
  const list = useAppSelector((state) => state.relation);

  useEffect(() => {
    handleReFetchAccepted();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <main className='main card'
      style={{
        flexDirection: 'column',
        width: "clamp(375px, 80vw, 800px)",
        flexGrow: "1"
      }}
    >
      {!isLoading && list["Accepted"].map((r) =>
        <ShowPerson r={r} key={r.id} from={"Accepted"} />
      )}
      {isLoading && <h1>Loading...</h1>}
    </main>
  )
}

export default Accepted