import React, { useState, useEffect } from 'react';
import axios from 'axios';
import SurveyQuestion from './SurveyQuestion';

const SurveyForm = ({ storeCode }) => {
    const [questions, setQuestions] = useState([]);
    const [responses, setResponses] = useState({});
    const [phoneNumber, setPhoneNumber] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [showPhoneVerification, setShowPhoneVerification] = useState(false); // Telefon doğrulama aşamasını kontrol etmek için yeni state

    useEffect(() => {
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
                userAgent: navigator.userAgent
            });
            setShowPhoneVerification(true); // Anket gönderildikten sonra telefon doğrulamasını göster
        } catch (error) {
            console.error("Gönderim hatası:", error);
            alert("Gönderim sırasında bir hata oluştu.");
        } finally {
            setIsSubmitting(false);
        }
    };

    const handlePhoneVerification = async () => {
        try {
            await axios.post('http://localhost:5139/api/verifyphone', { phoneNumber });
            alert("Telefon numaranız doğrulandı!");
        } catch (error) {
            console.error("Telefon doğrulama hatası:", error);
            alert("Telefon doğrulama sırasında bir hata oluştu.");
        }
    };

    if (showPhoneVerification) {
        return (
            <div>
                <label>Telefon Numarası:</label>
                <input
                    type="tel"
                    value={phoneNumber}
                    onChange={(e) => setPhoneNumber(e.target.value)}
                    required
                />
                <button onClick={handlePhoneVerification}>Telefonu Doğrula</button>
            </div>
        );
    }

    return (
        <form onSubmit={handleSubmit}>
            {questions.map(question => (
                <SurveyQuestion
                    key={question.surveyId}
                    question={question}
                    onResponseChange={handleResponseChange}
                />
            ))}
            <button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Gönderiliyor..." : "Gönder"}
            </button>
        </form>
    );
};

export default SurveyForm;
