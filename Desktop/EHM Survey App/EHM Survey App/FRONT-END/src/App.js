import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import SurveyForm from './SurveyForm';

function App() {
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
          <h1 style={{ color: '#1c45b0', fontSize: '3rem' }}>
            Müşteri Memnuniyeti Anketi
          </h1>
        </header>
        <main
          style={{
            flex: '1',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}
        >
          <Routes>
            <Route
              path="/"
              element={
                <p style={{ textAlign: 'center', fontSize: '1.5rem' }}>
                  Lütfen bulunduğunuz mağazanın QR kodunu okutun
                </p>
              }
            />
            <Route path="/survey/:StoreCode" element={<SurveyForm />} />
          </Routes>
        </main>
        <footer
          style={{
            textAlign: 'center',
            padding: '1%',
            backgroundColor: '#f1f1f1',
          }}
        >
          <img
            src="https://static.ticimax.cloud/61202/Uploads/HeaderTasarim/Header1/147893e0-e307-4754-8cc7-ec3849dac48f.jpg"
            alt="Logo"
            style={{ width: '20%', height: '20%' }}
          />
          <p>English Home © 2024</p>
        </footer>
      </div>
    </Router>
  );
}

export default App;
