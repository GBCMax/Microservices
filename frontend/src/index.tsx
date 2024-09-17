import ReactDOM from 'react-dom/client';
import './index.css';
import { App } from './App';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './context/authProvider';
import { ModalState } from './context/modalContext';
import { ConstantBackoff, WebsocketBuilder, WebsocketEvent } from 'websocket-ts';
import * as WsTypes from "./websocket/WsTypes";
import { ModalHello } from './context/modalHello';
import { useState } from 'react';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
const webSocket = new WebsocketBuilder('wss://localhost:7186/api/WebSocket/connect')
  .withBackoff(new ConstantBackoff(1000))
  .build();

webSocket.addEventListener(
  WebsocketEvent.message,
  async (_ws, _ev) => {
    await handleWsMsgAsync(_ev);
  },
);

async function handleWsMsgAsync(_ev: MessageEvent){
  const baseMsg = JSON.parse(_ev.data) as WsTypes.WsBaseMsg;

  if (baseMsg === undefined) {
      console.log(`Can't parse base ws msg: ${_ev.data}`);
      return;
  }

  if (baseMsg.type === 'ws-msg-hello') {
      const msgPayload: WsTypes.WsMsgHello = baseMsg.payload;
      
      console.log(msgPayload)
  }
}

root.render(
    <BrowserRouter>
      <AuthProvider>
        <ModalState>
          <App />
        </ModalState>
        </AuthProvider>
    </BrowserRouter>
);