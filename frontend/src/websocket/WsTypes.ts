export type WsBaseMsg = {
    type: 
      'ws-msg-hello' | 
      'ws-msg-default';
    payload: any;
  };
  
  export type WsMsgHello = {
    unixMs: number;
  };