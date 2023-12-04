import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import ChatPage from './Pages/ChatPage';
import StartPage from './Pages/StartPage';

function App() {
  return (
      <div>
          <BrowserRouter>
              <Routes>
                  <Route path='/' element={<StartPage />} />
                  <Route path='chat' element={<ChatPage />} />
              </Routes>
          </BrowserRouter>
    </div>
  );
}

export default App;
