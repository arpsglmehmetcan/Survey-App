import React, { useState, useEffect } from 'react';
import axios from 'axios';
import SurveyQuestion from './SurveyQuestion';

const SurveyForm = ({ storeCode }) => {
    const [questions, setQuestions] = useState([]);
    const [responses, setResponses] = useState({});
    const [phoneNumber, setPhoneNumber] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        // Store code’a göre soruları backend’den çek
        axios.get(`http://localhost:5139/api/survey/${storeCode}`)
            .then(response => setQuestions(response.data))
            .catch(error => console.error("Sorular yüklenirken hata oluştu:", error));
    }, [storeCode]);

    const handleResponseChange = (questionId, answer) => {
        setResponses(prevResponses => ({
            ...prevResponses,
            [questionId]: answer
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsSubmitting(true);
        try {
            await axios.post('http://localhost:5139/api/surveyresponse', {
                storeCode,
                responses,
                phoneNumber,
                userAgent: navigator.userAgent
            });
            alert("Anketiniz başarıyla gönderildi!");
        } catch (error) {
            console.error("Gönderim hatası:", error);
            alert("Gönderim sırasında bir hata oluştu.");
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            {questions.map(question => (
                <SurveyQuestion
                    key={question.surveyId}
                    question={question}
                    onResponseChange={handleResponseChange}
                />
            ))}
            <div>
                <label>Telefon Numarası:</label>
                <input
                    type="tel"
                    value={phoneNumber}
                    onChange={(e) => setPhoneNumber(e.target.value)}
                    required
                />
            </div>
            <button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Gönderiliyor..." : "Gönder"}
            </button>
        </form>
    );
};

export default SurveyForm;
