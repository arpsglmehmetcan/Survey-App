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
  const [isWaiting, setIsWaiting] = useState(false);
  const [timer, setTimer] = useState(120); // 2 dakika
  const [error, setError] = useState("");
  const [storeError, setStoreError] = useState("");
  const [windowWidth, setWindowWidth] = useState(window.innerWidth);

  const baseURL = "http://localhost:5139/api";

  useEffect(() => {
    const fetchQuestions = async () => {
      try {
        const response = await axios.get(`${baseURL}/survey/get-survey/${StoreCode}`);
        if (response.status === 200 && response.data.length > 0) {
          setStoreId(response.data[0].storeId);
          setQuestions(response.data);
        } else {
          setStoreError("Mağaza bulunamadı veya anket sorusu yok.");
        }
      } catch (error) {
        setStoreError("Bir hata oluştu. Mağaza bilgisi alınamadı.");
      }
    };

    fetchQuestions();

    const handleResize = () => {
      setWindowWidth(window.innerWidth);
    };

    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, [StoreCode]);

  /*
  useEffect(() => {
    if (isWaiting && timer > 0) {
      const interval = setInterval(() => {
        setTimer((prev) => prev - 1);
      }, 1000);
      return () => clearInterval(interval);
    } else if (timer === 0) {
      setIsWaiting(false);
      setTimer(120); // Süreyi sıfırla
    }
  }, [isWaiting, timer]);
  */

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

    if (!storeId) {
      setError("StoreId alınamadı. Lütfen sayfayı yenileyin.");
      return;
    }

    if (isWaiting) {
      setError("Lütfen 2 dakika bekleyiniz.");
      return;
    }

    try {
      const response = await axios.post(`${baseURL}/surveyresponse`, {
        Email: email,
        StoreCode,
        StoreId: storeId,
        Responses: JSON.stringify(values),
      });

      if (response.data.message) {
        setIsCodeSent(true);
        setIsWaiting(true);
        setTimer(120);
        alert(response.data.message);
      }
    } catch (error) {
      setError("Doğrulama kodu gönderilirken bir hata oluştu.");
    }
  };
      

  const handleVerifyCode = async () => {
    try {
      const response = await axios.post(`${baseURL}/surveyresponse/verify`, {
        Email: email,
        VerificationCode: verificationCode,
      });

      if (response.data.message) {
        alert("Doğrulama başarılı! Cevaplarınız kaydedildi.");
        navigate("/thank-you"); // Başarıyla doğrulandıktan sonra yönlendirme
      }
    } catch (error) {
      if (error.response && error.response.data.error) {
        setError(error.response.data.error); // Hata mesajını backend'den al
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
  };

  return (
    <div style={responsiveStyles.surveyContainer}>
      {storeError ? (
        <div style={responsiveStyles.errorMessage}>{storeError}</div>
      ) : (
        <Formik initialValues={generateInitialValues()} onSubmit={handleSendCode}>
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
                            //disabled={isWaiting} // Bekleme süresi varsa form elemanlarını devre dışı bırak
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
                      //disabled={isWaiting} // Bekleme süresi varsa form elemanlarını devre dışı bırak
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
                            //disabled={isWaiting} // Bekleme süresi varsa form elemanlarını devre dışı bırak
                          />
                          {option}
                        </label>
                      </div>
                    ))}
                </fieldset>
              ))}
              <div>
                <h3>E-posta Adresinizi Giriniz</h3>
                <input
                  type="text"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  style={responsiveStyles.input}
                  //disabled={isWaiting}
                />
                {isWaiting && <p>{`Kod gönderildi. Lütfen ${timer} saniye bekleyiniz.`}</p>}
                <button type="submit" style={responsiveStyles.button} /*disabled={isWaiting}*/>
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
                    style={responsiveStyles.input}
                  />
                  <button onClick={handleVerifyCode} style={responsiveStyles.button}>
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
