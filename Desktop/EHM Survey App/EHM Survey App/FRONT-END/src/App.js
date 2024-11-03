import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import SurveyForm from './SurveyForm';

function App() {
    return (
        <Router>
            <div className="App">
                <h1>Müşteri Memnuniyet Anketi</h1>
                <Routes>
                    <Route path="/survey/:StoreCode" element={<SurveyForm />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;
