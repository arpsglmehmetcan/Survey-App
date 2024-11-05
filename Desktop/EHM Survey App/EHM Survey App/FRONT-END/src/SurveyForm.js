import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import { Formik, Form, Field } from 'formik';

const SurveyForm = () => {
    const { StoreCode } = useParams();
    const [questions, setQuestions] = useState([]);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [showPhoneVerification, setShowPhoneVerification] = useState(false);
    const [PhoneNumber, setPhoneNumber] = useState('');

    const baseURL = 'http://192.168.1.33:5139/api';

    useEffect(() => {
        const fetchQuestions = async () => {
            try {
                const response = await axios.get(`${baseURL}/survey/get-surveys/${StoreCode}`);
                setQuestions(response.data);
            } catch (error) {
                console.error("Anket soruları yüklenirken hata oluştu:", error.response?.data || error.message);
                alert("Anket soruları yüklenirken bir hata oluştu. Lütfen tekrar deneyin.");
            }
        };
        fetchQuestions();
    }, [StoreCode]);

    const generateInitialValues = () => {
        const initialValues = {};
        questions.forEach(question => {
            initialValues[question.surveyId] = question.questionType === 'checkbox' ? [] : '';
        });
        return initialValues;
    };

    const handleSubmit = async (values) => {
        setIsSubmitting(true);
        try {
            await axios.post(`${baseURL}/surveyresponse`, {
                StoreCode,
                responses: values,
                UserAgent: navigator.userAgent
            });
            setShowPhoneVerification(true);
        } catch (error) {
            console.error("Gönderim sırasında hata oluştu:", error.response?.data || error.message);
            alert("Gönderim sırasında bir hata oluştu. Lütfen tekrar deneyin.");
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
            console.error("Telefon doğrulama sırasında hata oluştu:", error.response?.data || error.message);
            alert("Telefon doğrulama sırasında bir hata oluştu. Lütfen tekrar deneyin.");
        }
    };

    if (showPhoneVerification) {
        return (
            <div style={{ textAlign: 'center', marginTop: '20px' }}>
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
        <div style={{
            backgroundColor: '#1A374D',
            padding: '20px',
            height: '100%',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center'
        }}>
            <Formik
                initialValues={generateInitialValues()}
                enableReinitialize
                onSubmit={handleSubmit}
            >
                {({ values }) => (
                    <Form style={{
                        maxWidth: '100%',
                        width: '100%',
                        backgroundColor: '#406882',
                        padding: '20px',
                        borderRadius: '12px',
                        color: '#fff',
                        textAlign: 'left'
                    }}>
                        <h2 style={{ textAlign: 'center', marginBottom: '20px', color: '#fff' }}>Müşteri Memnuniyeti Anketi</h2>
                        {questions.map((question) => (
                            <div key={question.surveyId} style={{ marginBottom: '20px' }}>
                                <label style={{ display: 'block', fontWeight: 'bold', marginBottom: '10px', color: '#fff' }}>
                                    {question.question}
                                </label>
                                {question.questionType === 'radio' && question.questionOptions && JSON.parse(question.questionOptions).map((option, index) => (
                                    <div key={index} style={{ marginBottom: '5px' }}>
                                        <label style={{
                                            display: 'block',
                                            padding: '10px',
                                            borderRadius: '8px',
                                            backgroundColor: '#E0E1DD',
                                            cursor: 'pointer',
                                            color: '#000'
                                        }}>
                                            <Field type="radio" name={String(question.surveyId)} value={option} />
                                            {option}
                                        </label>
                                    </div>
                                ))}
                                {question.questionType === 'text' && (
                                    <Field
                                        type="text"
                                        name={String(question.surveyId)}
                                        placeholder="Cevabınızı girin..."
                                        style={{
                                            width: '100%',
                                            padding: '10px',
                                            borderRadius: '8px',
                                            border: '2px solid #ff5a5f',
                                            marginBottom: '10px'
                                        }}
                                    />
                                )}
                                {question.questionType === 'checkbox' && question.questionOptions && JSON.parse(question.questionOptions).map((option, index) => (
                                    <div key={index} style={{ marginBottom: '5px' }}>
                                        <label style={{
                                            display: 'block',
                                            padding: '10px',
                                            borderRadius: '8px',
                                            backgroundColor: '#E0E1DD',
                                            cursor: 'pointer',
                                            color: '#000'
                                        }}>
                                            <Field type="checkbox" name={`${question.surveyId}`} value={option} />
                                            {option}
                                        </label>
                                    </div>
                                ))}
                            </div>
                        ))}
                        <div style={{ textAlign: 'center', marginTop: '20px' }}>
                            <button
                                type="submit"
                                disabled={isSubmitting}
                                style={{
                                    padding: '10px 20px',
                                    backgroundColor: '#119DA4',
                                    color: '#fff',
                                    border: 'none',
                                    borderRadius: '8px',
                                    cursor: 'pointer',
                                    width: '100%'
                                }}
                            >
                                {isSubmitting ? "Gönderiliyor..." : "Gönder"}
                            </button>
                        </div>
                    </Form>
                )}
            </Formik>
        </div>
    );
    
};

export default SurveyForm;
