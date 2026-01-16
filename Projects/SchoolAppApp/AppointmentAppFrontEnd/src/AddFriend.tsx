import React from 'react'
import { FaSearch } from 'react-icons/fa';
import SearchPerson from './component/SearchPerson';
import useAddFriend from './hooks/useAddFriend';
import useCheckDirectAccessor from './hooks/useCheckDirectAccessor';

const AddFriend = () => {
  useCheckDirectAccessor();

  const { searchId, setSearchId, handleSearch, personInfo, handleRequest } = useAddFriend();

  return (
    <main className='main'
      style={{
        flexDirection: 'column',
        width: "clamp(375px, 80vw, 800px)",
      }}
    >
      <form id="SearchPost" className="card SearchPost__container" onSubmit={(e) => e.preventDefault()}>
        <input
          type="text"
          name="SearchContent"
          id="SearchContent"
          value={searchId}
          placeholder="Search Someone by Id..."
          required
          onChange={(e) => setSearchId(e.target.value)}
        />
        <button
          type="submit"
          className="flex"
          onClick={handleSearch}
        >
          <FaSearch />
        </button>
      </form>

      <SearchPerson personInfo={personInfo} handleRequest={handleRequest} />
    </main>
  )
}

export default AddFriend