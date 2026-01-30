import React from 'react'
import { RiAdminLine } from "react-icons/ri";
import { PiStudent } from "react-icons/pi";
import { FaChalkboardTeacher } from "react-icons/fa";
import { GoArchive } from "react-icons/go";
import useLoRContext from '../hooks/useContext';

function RoleChangeButton() {
  const { handleRoleButtonStyle, setRole } = useLoRContext();

  return (
    <>
      <button
        type="button"
        style={handleRoleButtonStyle("student")}
        className="role__button withTip"
        onClick={() => setRole("student")}
      >
        <PiStudent />
        <span className='tooltip up'>Login as student</span>
      </button>
      <button
        type="button"
        style={handleRoleButtonStyle("teacher")}
        className="role__button withTip"
        onClick={() => setRole("teacher")}
      >
        <FaChalkboardTeacher />
        <span className='tooltip up'>Login as teacher</span>
      </button>
      <button
        type="button"
        style={handleRoleButtonStyle("admin")}
        className="role__button withTip"
        onClick={() => setRole("admin")}
      >
        <RiAdminLine />
        <span className='tooltip up'>Login as admin</span>
      </button>
      <button
        type="button"
        style={handleRoleButtonStyle("schoolPrinciple")}
        className="role__button withTip"
        onClick={() => setRole("schoolPrinciple")}
      >
        <GoArchive />
        <span className='tooltip up'>Login as school principle</span>
      </button>
    </>
  )
}

export default RoleChangeButton