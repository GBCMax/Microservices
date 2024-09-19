export type WsBaseMsg = {
    type: 
      'ws-msg-hello' | 
      'ws-msg-speed-update';
    payload: any;
  };
  
  export type WsMsgHello = {
    unixMs: number;
  };

  export type WsMsgSpeedUpdate = {
    currentSpeed: number;
  };