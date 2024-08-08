import ReactDOM from 'react-dom/client';
import './index.css';
import { App } from './App';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './context/authProvider';
import { ModalState } from './context/modalContext';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
    <BrowserRouter>
      <AuthProvider>
        <ModalState>
          <App />
        </ModalState>
        </AuthProvider>
    </BrowserRouter>
);