import React from "react";
import JsonElement from "./JsonElement";

type Props = {
  item: any;
};

const JsonData = ({ item }: Props) => {
  return (
    <tr>
      {Object.keys(item).map((k) => (
        <JsonElement value={item[k as keyof typeof item]} />
      ))}
    </tr>
  );
};

export default JsonData;
