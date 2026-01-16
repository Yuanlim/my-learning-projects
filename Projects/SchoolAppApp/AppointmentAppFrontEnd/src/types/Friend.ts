export type PersonDataType = {
  id: string;
  userId: number;
  name: string;
};

export type FriendStatusType = "Accepted" | "Pending" | "Denied";
export type BlockCreatedType = {
  initiatorId: number;
  receiverId: number;
  blocked: boolean;
};

export type StatusRequestPayload = {
  ToUserId: number;
  Frps: FriendStatusType;
};
export type BlockRequestPayload = { ToUserId: number };
