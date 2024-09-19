import { ConstantBackoff, WebsocketBuilder } from "websocket-ts";

export const webSocket = new WebsocketBuilder('wss://localhost:7186/api/WebSocket/connect')
  .withBackoff(new ConstantBackoff(1000))
  .build();