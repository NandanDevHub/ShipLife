import * as signalR from "@microsoft/signalr";

const HUB = process.env.NEXT_PUBLIC_SIGNALR_URL || "http://localhost:5080/hubs/board";

export function connectHub(onEvent: (name: string, payload: any) => void) {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(HUB)
    .withAutomaticReconnect()
    .build();

  ["CardCreated","CardMoved","CardUpdated","CardDeleted","CommentAdded","AttachmentAdded"].forEach(evt => {
    connection.on(evt, (payload) => onEvent(evt, payload));
  });

  connection.start().catch(console.error);
  return connection;
}
