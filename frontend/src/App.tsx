import { ConstantBackoff, WebsocketBuilder, WebsocketEvent } from "websocket-ts";
import { Footer } from "./components/footer";
import { Navbar } from "./components/navbar";
import { useRoutes } from "./routes/routes";
import * as WsTypes from "./websocket/WsTypes"
import { useState } from "react";
import { ModalHello } from "./context/modalHello";
import axios from "axios";

const webSocket = new WebsocketBuilder('wss://localhost:7186/api/WebSocket/connect')
  .withBackoff(new ConstantBackoff(1000))
  .build();


export function App() {

  const routes = useRoutes()
  const [showHelloModal, setShowModalHello] = useState(false);

  webSocket.addEventListener(
    WebsocketEvent.message,
    async (_ws, _ev) => {
      await handleWsMsgAsync(_ev);
    },
  );

  async function GetSpeed(){
    const speed = await axios.get('https://localhost:7186/api/WebSocket/connectToCar')
    console.log(speed.data)
    setTimeout(GetSpeed, 5000);
  }

  GetSpeed()
  
  async function handleWsMsgAsync(_ev: MessageEvent){
    const baseMsg = JSON.parse(_ev.data) as WsTypes.WsBaseMsg;
  
    if (baseMsg === undefined) {
        console.log(`Can't parse base ws msg: ${_ev.data}`);
        return;
    }
  
    if (baseMsg.type === 'ws-msg-hello') {
        const msgPayload: WsTypes.WsMsgHello = baseMsg.payload;
        showHelloNotification();
        console.log(msgPayload)
    }

    if (baseMsg.type === 'ws-msg-speed-update') {
      const msgPayload: WsTypes.WsMsgSpeedUpdate = baseMsg.payload;
      console.log(msgPayload)
    }
  }

  const showHelloNotification = () => {
    setShowModalHello(true);
  }

  const closeModalHello = () => {
    setShowModalHello(false)
  }

  return (
    <>
      <Navbar />
      {showHelloModal && <ModalHello message="Привет, медвед" onClose={closeModalHello} />}
      {routes}
      <Footer/>
    </>
  )
}