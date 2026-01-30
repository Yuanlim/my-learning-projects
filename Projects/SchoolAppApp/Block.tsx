import React, { useEffect } from 'react'
import ShowPerson from './component/ShowPerson'
import { useAppSelector } from './hooks/useReduxHook';
import useCheckDirectAccessor from './hooks/useCheckDirectAccessor';
import { useBlockContext } from './hooks/useContext';

function Block() {
  useCheckDirectAccessor();
  const { handleReFetchBlock, isLoading } = useBlockContext();
  const list = useAppSelector((state) => state.relation);

  useEffect(() => {
    handleReFetchBlock();
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
      {!isLoading && list["Block"].map((r) =>
        <ShowPerson r={r} key={r.id} from={"Block"} />
      )}
      {isLoading && <h1>Loading...</h1>}
    </main>
  )
}

export default Block