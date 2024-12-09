import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import { Formik, Form, Field } from 'formik';

const SurveyForm = () => {
  const { StoreCode } = useParams(); // URL'den StoreCode'u alıyoruz
  const [storeId, setStoreId] = useState(null); // StoreId burada saklanacak
  const [questions, setQuestions] = useState([]);
  const [email, setEmail] = useState('');
  const [verificationCode, setVerificationCode] = useState('');
  const [isCodeSent, setIsCodeSent] = useState(false);
  const [isVerified, setIsVerified] = useState(false);
  const [error, setError] = useState('');
  const [storeError, setStoreError] = useState('');
  const [windowWidth, setWindowWidth] = useState(window.innerWidth);

  const baseURL = 'http://localhost:5139/api';

  useEffect(() => {
    const fetchQuestions = async () => {
      try {
        // Store bilgilerini ve soruları alıyoruz
        const response = await axios.get(`${baseURL}/survey/get-survey/${StoreCode}`);
        if (response.status === 200 && response.data.length > 0) {
          setStoreId(response.data[0].storeId); // İlk anketin StoreId'sini set ediyoruz
          setQuestions(response.data); // Soruları set ediyoruz
        } else {
          setStoreError('Mağaza bulunamadı veya anket sorusu yok.');
        }
      } catch (error) {
        setStoreError('Bir hata oluştu. Mağaza bilgisi alınamadı.');
      }
    };

    fetchQuestions();

    // Ekran boyut değişikliklerini izlemek için event listener ekleme
    const handleResize = () => {
      setWindowWidth(window.innerWidth);
    };

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, [StoreCode]);

  const generateInitialValues = () => {
    const initialValues = {};
    questions.forEach((q) => {
      initialValues[q.surveyId] = q.questionType === 'checkbox' ? [] : '';
    });
    return initialValues;
  };

  const handleEmailChange = (e) => {
    setEmail(e.target.value);
  };

  const handleSendCode = async (values) => {
    if (!email || !/\S+@\S+\.\S+/.test(email)) {
      showError('Lütfen geçerli bir e-posta adresi giriniz.');
      return;
    }

    if (!storeId) {
      showError('StoreId alınamadı. Lütfen sayfayı yenileyin.');
      return;
    }

    try {
      const response = await axios.post(`${baseURL}/surveyresponse`, {
        Email: email,
        StoreCode, // URL'den alınan StoreCode
        StoreId: storeId, // Backend'den alınan StoreId
        Responses: JSON.stringify(values), // Yanıtlar JSON formatında
      });

      if (response.data.message) {
        setIsCodeSent(true);
        alert(response.data.message);
      }
    } catch (error) {
      showError('Doğrulama kodu gönderilirken bir hata oluştu.');
    }
  };

  const handleVerifyCode = async () => {
    try {
      const response = await axios.post(`${baseURL}/surveyresponse/verify`, {
        Email: email,
        VerificationCode: verificationCode,
      });

      if (response.data.message) {
        setIsVerified(true);
        alert('Doğrulama başarılı! Cevaplarınız kaydedildi.');
      } else {
        showError('Doğrulama kodu hatalı.');
      }
    } catch (error) {
      showError('Doğrulama sırasında hata oluştu.');
    }
  };

  const showError = (message) => {
    setError(message);
    setTimeout(() => {
      setError('');
    }, 3000);
  };

  const responsiveStyles = {
    surveyContainer: {
      padding: '40px',
      display: 'flex',
      justifyContent: 'center',
      flexDirection: 'column',
      alignItems: 'center',
      position: 'relative',
    },
    surveyForm: {
      maxWidth: windowWidth > 768 ? '500px' : '90%',
      backgroundColor: '#f1f1f1',
      padding: '20px',
      borderRadius: '8px',
      boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)',
    },
    label: {
      fontWeight: 'bold',
      display: 'block',
      marginBottom: '10px',
    },
    input: {
      width: '100%',
      padding: '12px',
      marginBottom: '15px',
      border: '1px solid #ddd',
      borderRadius: '5px',
    },
    button: {
      width: '100%',
      padding: '12px',
      backgroundColor: '#28a745',
      color: '#fff',
      border: 'none',
      borderRadius: '5px',
      cursor: 'pointer',
      marginTop: '10px',
    },
    errorMessage: {
      color: 'red',
      fontWeight: 'bold',
      textAlign: 'center',
      marginBottom: '10px',
      border: '1px solid red',
      padding: '10px',
      borderRadius: '5px',
      backgroundColor: '#ffe5e5',
    },
    h2: {
      color: '#1c45b0',
      textAlign: 'center',
      marginBottom: '20px',
    },
  };

  return (
    <div style={responsiveStyles.surveyContainer}>
      {storeError ? (
        <div style={responsiveStyles.errorMessage}>{storeError}</div>
      ) : (
        <Formik
          initialValues={generateInitialValues()}
          onSubmit={handleSendCode}
          enableReinitialize
        >
          {({ values }) => (
            <Form style={responsiveStyles.surveyForm}>
              <h2 style={responsiveStyles.h2}>Anket Soruları</h2>
              {questions.map((question) => (
                <fieldset key={question.surveyId}>
                  <label style={responsiveStyles.label}>
                    {question.question}
                  </label>
                  {question.questionType === 'radio' &&
                    question.questionOptions &&
                    JSON.parse(question.questionOptions).map((option, index) => (
                      <div key={index}>
                        <label>
                          <Field
                            type="radio"
                            name={String(question.surveyId)}
                            value={option}
                          />
                          {option}
                        </label>
                      </div>
                    ))}

                  {question.questionType === 'text' && (
                    <Field
                      type="text"
                      name={String(question.surveyId)}
                      placeholder="Cevabınızı yazın..."
                      style={responsiveStyles.input}
                    />
                  )}

                  {question.questionType === 'checkbox' &&
                    question.questionOptions &&
                    JSON.parse(question.questionOptions).map((option, index) => (
                      <div key={index}>
                        <label>
                          <Field
                            type="checkbox"
                            name={`${question.surveyId}`}
                            value={option}
                          />
                          {option}
                        </label>
                      </div>
                    ))}

                  {question.questionType === 'rating' && question.questionOptions && (
                    <Field
                      type="number"
                      name={String(question.surveyId)}
                      min={JSON.parse(question.questionOptions).min}
                      max={JSON.parse(question.questionOptions).max}
                      style={responsiveStyles.input2}
                    />
                  )}
                </fieldset>
              ))}

              <div>
                <h3>E-posta Adresinizi Giriniz</h3>
                <input
                  type="email"
                  value={email}
                  onChange={handleEmailChange}
                  placeholder="E-posta adresiniz"
                  required
                  style={responsiveStyles.input}
                />
                {error && (
                  <div style={responsiveStyles.errorMessage}>{error}</div>
                )}
                <button type="submit" style={responsiveStyles.button}>
                  Doğrulama Kodu Gönder
                </button>
              </div>

              {isCodeSent && (
                <>
                  <h3>Doğrulama Kodunu Giriniz</h3>
                  <input
                    type="text"
                    value={verificationCode}
                    onChange={(e) => setVerificationCode(e.target.value)}
                    placeholder="Doğrulama Kodu"
                    style={responsiveStyles.input}
                  />
                  <button
                    onClick={handleVerifyCode}
                    style={responsiveStyles.button}
                  >
                    Doğrula ve Gönder
                  </button>
                </>
              )}
            </Form>
          )}
        </Formik>
      )}
    </div>
  );
};

export default SurveyForm;
