import React from 'react';

const SurveyQuestion = ({ Question, onResponseChange }) => {
  const handleChange = (e) => {
    onResponseChange(Question.SurveyId, e.target.value);
  };

  return (
    <div>
      <label>{Question.Question}</label>
      {Question.QuestionType === 'radio' && (
        <div>
          {Question.options.map((option, index) => (
            <label key={index}>
              <input
                type="radio"
                value={option}
                name={Question.SurveyId}
                onChange={handleChange}
              />
              {option}
            </label>
          ))}
        </div>
      )}
      {Question.QuestionType === 'checkbox' && (
        <div>
          {Question.options.map((option, index) => (
            <label key={index}>
              <input
                type="checkbox"
                value={option}
                name={Question.SurveyId}
                onChange={(e) => {
                  const selectedOptions = Question.selectedOptions || [];
                  if (e.target.checked) {
                    onResponseChange(Question.SurveyId, [
                      ...selectedOptions,
                      option,
                    ]);
                  } else {
                    onResponseChange(
                      Question.SurveyId,
                      selectedOptions.filter((item) => item !== option)
                    );
                  }
                }}
              />
              {option}
            </label>
          ))}
        </div>
      )}
      {Question.QuestionType === 'rating' && (
        <input 
        type="number" 
        min={JSON.parse(Question.QuestionOptions).min}
        max={JSON.parse(Question.QuestionOptions).max} 
        onChange={handleChange} 
        />
      )}
      {Question.QuestionType === 'text' && (
        <input type="text" onChange={handleChange} />
      )}
    </div>
  );
};

export default SurveyQuestion;