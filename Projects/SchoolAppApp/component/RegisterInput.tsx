import React from "react";
import useLoRContext from "../hooks/useContext";

const RegisterInput = () => {
  const { email, setEmail, name, setName, phoneNumber, setPhoneNumber, schoolClass, setSchoolClass, role } = useLoRContext();

  return (
    <>
      <div className="registerInput_container">
        <label htmlFor="Email" className="LoR__label">Email</label>
        <input
          type="email"
          name="Email"
          id="Email"
          value={email}
          className="LoR__input"
          required
          onChange={(e) => {
            setEmail(e.target.value);
          }}
        />
      </div>
      <div className="registerInput_container">
        <label htmlFor="Name" className="LoR__label">Name</label>
        <input
          type="text"
          name="Name"
          id="Name"
          value={name}
          className="LoR__input"
          required
          onChange={(e) => {
            setName(e.target.value);
          }}
        />
      </div>
      <div className="registerInput_container">
        <label htmlFor="Phone" className="LoR__label">Phone</label>
        <input
          type="text"
          name="Phone"
          id="Phone"
          value={phoneNumber}
          className="LoR__input"
          required
          onChange={(e) => {
            setPhoneNumber(e.target.value);
          }}
        />
      </div>
      {role === "student" && <div className="registerInput_container">
        <label htmlFor="SchoolClass" className="LoR__label">Class</label>
        <select
          name="SchoolClass"
          id="SchoolClass"
          value={schoolClass}
          className="class__selector"
          onChange={(e) => {
            setSchoolClass(e.target.value);
          }}
        >
          <option value="電通一甲">電通一甲</option>
          <option value="電通二甲">電通二甲</option>
          <option value="電通三甲">電通三甲</option>
        </select>
      </div>}
    </>
  );
};

export default RegisterInput;
