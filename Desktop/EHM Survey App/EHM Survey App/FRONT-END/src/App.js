import React from 'react';
import SurveyForm from './SurveyForm';

function App() {
    return (
        <div className="App">
            <h1>Müşteri Memnuniyet Anketi</h1>
            <SurveyForm storeCode="STORE123" />
        </div>
    );
}

export default App;
