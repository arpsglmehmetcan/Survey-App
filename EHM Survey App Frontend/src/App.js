import React, { useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import SurveyForm from './SurveyForm';
import ThankYouPage from "./ThankYouPage";
import SurveyEdit from './SurveyEdit';
import LoginPage from './LoginPage';
import AdminPanel from "./AdminPanel";
import SurveyResults from "./SurveyResults";
import UserEdit from './UserEdit';

function App() {
  /*useEffect(() => {
    const disableRightClick = (e) => e.preventDefault();
    document.addEventListener('contextmenu', disableRightClick);

    const disableShortcuts = (e) => {
      if (
        (e.ctrlKey &&
          (e.key === 'u' || e.key === 'U' || e.key === 's' || e.key === 'S')) ||
        e.key === 'F12' ||
        (e.ctrlKey && e.shiftKey && e.key === 'I')
      ) {
        e.preventDefault();
      }
    };
    document.addEventListener('keydown', disableShortcuts);

    return () => {
      document.removeEventListener('contextmenu', disableRightClick);
      document.removeEventListener('keydown', disableShortcuts);
    };
  }, []);   */

  return (
    <Router>
      <div
        className="App"
        style={{
          minHeight: '100vh',
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
        }}
      >
        <header style={{ textAlign: 'center', padding: '1%' }}>
          <h1
            style={{ color: '#1c45b0', fontSize: 'clamp(1.5rem, 5vw, 3rem)' }}
          >
            Müşteri Memnuniyeti Anketi
          </h1>
        </header>
        <main
          style={{
            flex: '1',
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'flex-start',
          }}
        >
          <Routes>
            <Route
              path="/"
              element={
                <p style={{ 
                  textAlign: 'center',
                   fontSize: '1.5rem',
                   marginTop: '15%',
                   }}>
                  Lütfen bulunduğunuz mağazanın QR kodunu okutun
                </p>
              }
            />
            <Route path="/survey/:StoreCode" element={<SurveyForm />} />
            <Route path="/thank-you" element={<ThankYouPage />} />
            <Route path="/survey-edit/:StoreCode" element={<SurveyEdit />} />
            <Route path="/survey-results" element={<SurveyResults />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/admin-panel" element={<AdminPanel />} />
            <Route path="/user-edit" element={<UserEdit />} />
          </Routes>
        </main>
        <footer
          style={{
            textAlign: 'center',
            padding: '0',
            margin: '0',
            bottom: '0',
            width: '100%',
            //position: 'fixed',
            backgroundColor: '#f1f1f1',
            pointerEvents: 'none',
            userSelect: 'none',
          }}
        >
          <img
            src="https://static.ticimax.cloud/61202/Uploads/HeaderTasarim/Header1/147893e0-e307-4754-8cc7-ec3849dac48f.jpg"
            alt="Logo"
            style={{ width: '20%', height: '20%', paddingTop: '2%' }}
            draggable="false"
            onContextMenu={(e) => e.preventDefault()}
          />
          <p>English Home © 2024</p>
        </footer>
      </div>
    </Router>
  );
}

export default App;
