import { Dispatch, SetStateAction } from "react";
import { PostType } from "./posts";

export type StringStateFuncType = Dispatch<SetStateAction<string>>;

export type BoolStateFuncType = Dispatch<SetStateAction<boolean>>;

export type PostArrayStateFuncType = Dispatch<SetStateAction<PostType[]>>;
