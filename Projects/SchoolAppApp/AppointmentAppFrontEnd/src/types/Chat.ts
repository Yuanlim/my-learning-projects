export type ChatMessage = {
  iSended: boolean;
  content: string;
  postDateTime: string;
};

export type ResponseChatMessage = {
  lastMessageId: number;
  chatMessages: ChatMessage[];
};

export type GetChatPayload = {
  OthersId: number;
  LastMessageId: number;
};

export type PostChatPayload = {
  ToUserId?: number;
  Content?: string;
};
