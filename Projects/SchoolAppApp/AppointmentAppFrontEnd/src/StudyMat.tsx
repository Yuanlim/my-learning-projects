import React from 'react'
import useCheckDirectAccessor from './hooks/useCheckDirectAccessor';

type Props = {}

const StudyMat = (props: Props) => {
  useCheckDirectAccessor();
  return (
    <main className='main'>
      <div>StudyMat</div>
    </main>
  )
}

export default StudyMat