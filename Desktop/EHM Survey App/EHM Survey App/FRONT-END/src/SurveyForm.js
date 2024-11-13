import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import { Formik, Form, Field } from 'formik';

const SurveyForm = () => {
    const { StoreCode } = useParams();
    const [questions, setQuestions] = useState([]);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const baseURL = 'http://192.168.1.33:5139/api';

    useEffect(() => {
        const fetchQuestions = async () => {
            try {
                const response = await axios.get(`${baseURL}/survey/get-surveys/${StoreCode}`);
                setQuestions(response.data);
            } catch (error) {
                console.error("Anket soruları yüklenirken hata oluştu:", error);
            }
        };
        fetchQuestions();
    }, [StoreCode]);

    const generateInitialValues = () => {
        const initialValues = {};
        questions.forEach(q => {
            initialValues[q.surveyId] = q.questionType === 'checkbox' ? [] : '';
        });
        return initialValues;
    };

    const handleSubmit = async (values) => {
        setIsSubmitting(true);
        try {
            await axios.post(`${baseURL}/surveyresponse`, {
                StoreCode,
                responses: values
            });
            alert("Anket başarıyla gönderildi!");
        } catch (error) {
            console.error("Gönderim sırasında hata oluştu:", error);
            alert("Gönderim sırasında bir hata oluştu.");
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div style={{ padding: '40px', display: 'flex', justifyContent: 'center' }}>
            <Formik initialValues={generateInitialValues()} onSubmit={handleSubmit} enableReinitialize>
                {({ values }) => (
                    <Form>
                        <h2 className="heading">Anket Soruları</h2>
                        {questions.map((question) => (
                            <fieldset key={question.surveyId} style={{ marginBottom: '20px' }}>
                                <label>{question.question}</label>
                                {question.questionType === 'radio' && question.questionOptions &&
                                    JSON.parse(question.questionOptions).map((option, index) => (
                                        <div className="radio-group" key={index}>
                                            <label>
                                                <Field type="radio" name={String(question.surveyId)} value={option} />
                                                {option}
                                            </label>
                                        </div>
                                    ))}
                                {question.questionType === 'text' && (
                                    <Field
                                        type="text"
                                        name={String(question.surveyId)}
                                        placeholder="Cevabınızı yazın..."
                                    />
                                )}
                                {question.questionType === 'checkbox' && question.questionOptions &&
                                    JSON.parse(question.questionOptions).map((option, index) => (
                                        <div className="checkbox-group" key={index}>
                                            <label>
                                                <Field type="checkbox" name={`${question.surveyId}`} value={option} />
                                                {option}
                                            </label>
                                        </div>
                                    ))}
                            </fieldset>
                        ))}
                        <button type="submit" disabled={isSubmitting}>
                            {isSubmitting ? "Gönderiliyor..." : "Gönder"}
                        </button>
                    </Form>
                )}
            </Formik>
        </div>
    );
};

export default SurveyForm;
