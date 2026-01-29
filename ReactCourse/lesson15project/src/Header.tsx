import React from "react";
import Button from "./Button";

type Props = {
  currentRequest: string;
  handleChangeUrl: (request: string) => void;
};

function Header({ currentRequest, handleChangeUrl }: Props) {
  return (
    <form onSubmit={(e) => e.preventDefault()}>
      {["users", "posts", "comments"].map((r, i) => (
        <Button
          currentRequest={currentRequest}
          request={r}
          handleChangeUrl={handleChangeUrl}
          key={i}
        />
      ))}
    </form>
  );
}

export default Header;
