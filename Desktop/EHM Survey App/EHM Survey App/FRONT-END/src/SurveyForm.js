import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import { Formik, Form, Field } from 'formik';
import InputMask from 'react-input-mask';

const SurveyForm = () => {
  const { StoreCode } = useParams();
  const [questions, setQuestions] = useState([]);
  const [phoneNumber, setPhoneNumber] = useState('+90 ');
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
        const response = await axios.get(
          `${baseURL}/survey/get-survey/${StoreCode}`
        );
        setQuestions(response.data);
      } catch (error) {
        if (
          error.response &&
          error.response.data &&
          error.response.data.message
        ) {
          setStoreError(error.response.data.message);
        } else {
          setStoreError('Beklenmeyen bir hata oluştu.');
        }
      }
    };
    fetchQuestions();

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

  const handlePhoneChange = (e) => {
    let value = e.target.value;
    if (!value.startsWith('+90 ')) {
      value = '+90 ';
    }
    setPhoneNumber(value);
  };

  const handleSendCode = async (values) => {
    if (phoneNumber.replace(/\D/g, '').length !== 12) {
      showError('Lütfen geçerli bir telefon numarası giriniz.');
      return;
    }

    try {
      const response = await axios.post(`${baseURL}/surveyresponse`, {
        PhoneNumber: phoneNumber,
        StoreCode,
        Responses: values,
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
        PhoneNumber: phoneNumber,
        VerificationCode: verificationCode,
      });
      if (response.data === 'SMS doğrulandı ve sonuç kaydedildi.') {
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
    input2: {
      width: '20%',
      padding: '12px',
      marginBottom: '15px',
      border: '1px solid #ddd',
      borderRadius: '5px',
    },
    phoneInput: {
      width: windowWidth > 768 ? '120px' : '100%',
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
                <h3>Telefon Numaranızı Giriniz</h3>
                <InputMask
                  mask="+90 999 999 99 99"
                  value={phoneNumber}
                  onChange={handlePhoneChange}
                  placeholder="+90 501 234 56 78"
                  required
                  maskChar={null}
                  style={responsiveStyles.phoneInput}
                />
                {error && (
                  <div style={responsiveStyles.errorMessage}>{error}</div>
                )}
                <button type="submit" style={responsiveStyles.button}>
                  SMS Kodu Gönder
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
