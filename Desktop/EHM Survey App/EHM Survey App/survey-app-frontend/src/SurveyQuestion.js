// src/SurveyQuestion.js
import React from 'react';

const SurveyQuestion = ({ question, onResponseChange }) => {
    const handleChange = (e) => {
        onResponseChange(question.surveyId, e.target.value);
    };

    return (
        <div>
            <label>{question.question}</label>
            {question.questionType === 'radio' ? (
                <div>
                    {/* Varsayım: Radio button seçenekleri (Evet, Hayır) */}
                    <label>
                        <input type="radio" value="Evet" name={question.surveyId} onChange={handleChange} /> Evet
                    </label>
                    <label>
                        <input type="radio" value="Hayır" name={question.surveyId} onChange={handleChange} /> Hayır
                    </label>
                </div>
            ) : (
                <input type="text" onChange={handleChange} />
            )}
        </div>
    );
};

export default SurveyQuestion;
