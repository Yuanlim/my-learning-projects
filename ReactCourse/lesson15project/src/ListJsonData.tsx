import React from "react";
import JsonData from "./JsonData";

type Props = { data: unknown };

const ListJsonData = ({ data }: Props) => {
  return (
    <table>
      <thead>
        <tr>
          {Array.isArray(data) && Object.keys(data[0]).map((k) => <th>{k}</th>)}
        </tr>
      </thead>
      <tbody>
        {Array.isArray(data) && data.map((item) => <JsonData item={item} />)}
      </tbody>
    </table>
  );
};

export default ListJsonData;
