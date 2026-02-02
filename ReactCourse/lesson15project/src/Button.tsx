import React from "react";

type Props = {
  currentRequest: string;
  request: string;
  handleChangeUrl: (request: string) => void;
};

function Button({ currentRequest, request, handleChangeUrl }: Props) {
  return (
    <button
      type="submit"
      className={currentRequest === request ? "selected" : undefined}
      onClick={() => handleChangeUrl(request)}
    >
      {request}
    </button>
  );
}

export default Button;
