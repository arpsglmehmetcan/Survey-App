import React from 'react';

const SurveyQuestion = ({ question, onResponseChange }) => {
    const handleChange = (e) => {
        onResponseChange(question.surveyId, e.target.value);
    };

    return (
        <div>
            <label>{question.question}</label>
            {question.questionType === 'radio' && (
                <div>
                    {question.options.map((option, index) => (
                        <label key={index}>
                            <input
                                type="radio"
                                value={option}
                                name={question.surveyId}
                                onChange={handleChange}
                            />
                            {option}
                        </label>
                    ))}
                </div>
            )}
            {question.questionType === 'checkbox' && (
                <div>
                    {question.options.map((option, index) => (
                        <label key={index}>
                            <input
                                type="checkbox"
                                value={option}
                                name={question.surveyId}
                                onChange={(e) => {
                                    const selectedOptions = responses[question.surveyId] || [];
                                    if (e.target.checked) {
                                        onResponseChange(question.surveyId, [...selectedOptions, option]);
                                    } else {
                                        onResponseChange(question.surveyId, selectedOptions.filter(item => item !== option));
                                    }
                                }}
                            />
                            {option}
                        </label>
                    ))}
                </div>
            )}
            {question.questionType === 'rating' && (
                <input
                    type="number"
                    min="1"
                    max="10"
                    onChange={handleChange}
                />
            )}
            {question.questionType === 'text' && (
                <input
                    type="text"
                    onChange={handleChange}
                />
            )}
        </div>
    );
};

export default SurveyQuestion;
