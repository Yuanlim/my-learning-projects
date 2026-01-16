import React, { useEffect } from 'react'
import { useAppSelector } from './hooks/useReduxHook'
import ShowPerson from './component/ShowPerson';
import useCheckDirectAccessor from './hooks/useCheckDirectAccessor';
import { usePendingContext } from './hooks/useContext';

function Pending() {
  useCheckDirectAccessor();
  const { handleReFetchPending, isLoading } = usePendingContext();
  const list = useAppSelector((state) => state.relation);

  useEffect(() => {
    handleReFetchPending();
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
      {!isLoading && list["Pending"].map((r) =>
        <ShowPerson r={r} key={r.id} from={"Pending"} />
      )}
      {isLoading && <h1>Loading...</h1>}
    </main>
  )
}

export default Pending