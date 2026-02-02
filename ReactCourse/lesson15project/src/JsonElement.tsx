import React from "react";

type Props = { value: any };

const JsonElement = ({ value }: Props) => {
  return (
    <td>
      <p>{JSON.stringify(value)}</p>
    </td>
  );
};

export default JsonElement;
