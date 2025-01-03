import React, { useState, useEffect } from "react";
import axios from "axios";
import { useParams, useNavigate } from "react-router-dom";
import { Formik, Form, Field } from "formik";

const SurveyForm = () => {
  const { StoreCode } = useParams();
  const navigate = useNavigate();
  const [storeId, setStoreId] = useState(null);
  const [questions, setQuestions] = useState([]);
  const [email, setEmail] = useState("");
  const [verificationCode, setVerificationCode] = useState("");
  const [isCodeSent, setIsCodeSent] = useState(false);
  const [error, setError] = useState("");
  const [storeError, setStoreError] = useState("");
  const [windowWidth, setWindowWidth] = useState(window.innerWidth);

  const baseURL = "http://localhost:5139/api";

  useEffect(() => {
    const fetchActiveQuestions = async () => {
      try {
        const response = await axios.get(`${baseURL}/survey/get-active-survey/${StoreCode}`);
        if (response.status === 200 && response.data.length > 0) {
          setStoreId(response.data[0].storeId);
          setQuestions(response.data); // Sadece aktif soruları al
        } else {
          setStoreError("Mağaza bulunamadı veya aktif anket sorusu yok.");
        }
      } catch (error) {
        setStoreError("Bir hata oluştu. Mağaza bilgisi alınamadı.");
      }
    };

    fetchActiveQuestions();

    const handleResize = () => {
      setWindowWidth(window.innerWidth);
    };

    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, [StoreCode]);

  const generateInitialValues = () => {
    const initialValues = {};
    questions.forEach((q) => {
      initialValues[q.surveyId] = q.questionType === "checkbox" ? [] : "";
    });
    return initialValues;
  };

  const validateEmail = (email) => {
    const emailRegex = /^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailRegex.test(email);
  };

  const handleSendCode = async (values) => {
    if (!email || !validateEmail(email)) {
      setError("Lütfen geçerli bir e-posta adresi giriniz.");
      return;
    }

    // Yanıtları JSON formatına dönüştür
    const responses = questions.reduce((acc, question) => {
      acc[question.surveyId] = values[question.surveyId];
      return acc;
    }, {});

    try {
      const response = await axios.post(`${baseURL}/surveyresponse/send-code`, {
        Email: email,
        StoreId: storeId,
        Responses: JSON.stringify(responses),
      });

      if (response.data.message) {
        setIsCodeSent(true);
        alert(response.data.message);
      }
    } catch (error) {
      if (error.response && error.response.data.error) {
        setError(error.response.data.error);
      } else {
        setError("Doğrulama kodu gönderilirken bir hata oluştu.");
      }
    }
  };

  const handleVerifyCode = async (values) => {
    // Yanıtları JSON formatına dönüştür
    const responses = questions.reduce((acc, question) => {
      acc[question.surveyId] = values[question.surveyId];
      return acc;
    }, {});

    try {
      const response = await axios.post(`${baseURL}/surveyresponse/verify`, {
        Email: email,
        VerificationCode: verificationCode,
        Responses: JSON.stringify(responses),
      });

      if (response.data.message) {
        navigate("/thank-you");
      }
    } catch (error) {
      if (error.response && error.response.data.error) {
        setError(error.response.data.error);
      } else {
        setError("Doğrulama sırasında bir hata oluştu.");
      }
    }
  };

  const responsiveStyles = {
    surveyContainer: {
      padding: "40px",
      display: "flex",
      justifyContent: "center",
      flexDirection: "column",
      alignItems: "center",
    },
    surveyForm: {
      maxWidth: windowWidth > 768 ? "500px" : "90%",
      backgroundColor: "#f1f1f1",
      padding: "20px",
      borderRadius: "8px",
      boxShadow: "0 4px 8px rgba(0, 0, 0, 0.1)",
    },
    label: {
      fontWeight: "bold",
      display: "block",
      marginBottom: "10px",
    },
    input: {
      width: "100%",
      padding: "12px",
      marginBottom: "15px",
      border: "1px solid #ddd",
      borderRadius: "5px",
    },
    button: {
      width: "100%",
      padding: "12px",
      backgroundColor: "#28a745",
      color: "#fff",
      border: "none",
      borderRadius: "5px",
      cursor: "pointer",
      marginTop: "10px",
    },
    errorMessage: {
      color: "red",
      fontWeight: "bold",
      textAlign: "center",
      marginBottom: "10px",
    },
    h2: {
      color: "#1c45b0",
      textAlign: "center",
      marginBottom: "20px",
    },
    input2: {
      width: "20%",
      padding: "12px",
      marginBottom: "15px",
      border: "1px solid #ddd",
      borderRadius: "5px",
    },
  };

  return (
    <div style={responsiveStyles.surveyContainer}>
      {storeError ? (
        <div style={responsiveStyles.errorMessage}>{storeError}</div>
      ) : (
        <Formik
          initialValues={generateInitialValues()}
          onSubmit={isCodeSent ? handleVerifyCode : handleSendCode}
        >
          {({ values }) => (
            <Form style={responsiveStyles.surveyForm}>
              <h2 style={responsiveStyles.h2}>Anket Soruları</h2>
              {questions.map((question) => (
                <fieldset key={question.surveyId}>
                  <label style={responsiveStyles.label}>{question.question}</label>
                  {question.questionType === "radio" &&
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
                  {question.questionType === "text" && (
                    <Field
                      type="text"
                      name={String(question.surveyId)}
                      style={responsiveStyles.input}
                    />
                  )}
                  {question.questionType === "checkbox" &&
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
                  {question.questionType === "rating" && (
                    <Field
                      type="number"
                      name={String(question.surveyId)}
                      min={question.questionOptions ? JSON.parse(question.questionOptions).min : 1}
                      max={question.questionOptions ? JSON.parse(question.questionOptions).max : 10}
                      style={responsiveStyles.input2}
                    />
                  )}
                </fieldset>
              ))}
              <div>
                <h3>E-posta Adresinizi Giriniz</h3>
                <input
                  type="text"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  style={responsiveStyles.input}
                />
                <button type="submit" style={responsiveStyles.button}>
                  {isCodeSent ? "Doğrula ve Gönder" : "Doğrulama Kodu Gönder"}
                </button>
              </div>
              {isCodeSent && (
                <div>
                  <h3>Doğrulama Kodunu Giriniz</h3>
                  <input
                    type="text"
                    value={verificationCode}
                    onChange={(e) => setVerificationCode(e.target.value)}
                    style={responsiveStyles.input}
                  />
                </div>
              )}
            </Form>
          )}
        </Formik>
      )}
    </div>
  );
};

export default SurveyForm;
