import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import SurveyQuestion from './SurveyQuestion';

const SurveyForm = () => {
    const { StoreCode } = useParams();
    console.log('StoreCode:', StoreCode); // Konsola yazdırarak kontrol edin
    const [questions, setQuestions] = useState([]);
    const [responses, setResponses] = useState({});
    const [PhoneNumber, setPhoneNumber] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [showPhoneVerification, setShowPhoneVerification] = useState(false);

    const baseURL = 'http://192.168.1.33:5139/api';

    useEffect(() => {
        const fetchQuestions = async () => {
            try {
                const Response = await axios.get(`${baseURL}/survey/get-surveys/${StoreCode}`);
                setQuestions(Response.data);
                console.log("Anket Soruları:", Response.data); // Anket sorularını da kontrol edin
            } catch (error) {
                console.error("Sorular yüklenirken hata oluştu:", error);
            }
        };
        fetchQuestions();
    }, [StoreCode]);

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
            await axios.post(`${baseURL}/surveyresponse`, {
                StoreCode,
                responses,
                UserAgent: navigator.UserAgent
            });
            setShowPhoneVerification(true);
        } catch (error) {
            console.error("Gönderim hatası:", error);
            alert("Gönderim sırasında bir hata oluştu.");
        } finally {
            setIsSubmitting(false);
        }
    };

    const handlePhoneVerification = async () => {
        try {
            await axios.post(`${baseURL}/surveyresponse/verify`, { PhoneNumber });
            alert("Telefon numaranız doğrulandı!");
            setShowPhoneVerification(false);
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
                    value={PhoneNumber}
                    onChange={(e) => setPhoneNumber(e.target.value)}
                    required
                />
                <button onClick={handlePhoneVerification}>Telefonu Doğrula</button>
            </div>
        );
    }

    return (
        <form onSubmit={handleSubmit}>
            {questions.map(Question => (
                <SurveyQuestion
                    key={Question.SurveyId}
                    Question={Question}
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
