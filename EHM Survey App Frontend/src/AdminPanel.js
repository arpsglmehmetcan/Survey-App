import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const AdminPanel = () => {
  const navigate = useNavigate();

  // Kullanıcı bilgilerini state'te tut
  const [userData, setUserData] = useState({
    userName: "",
    storeId: null,
  });

  useEffect(() => {
    // LocalStorage'dan kullanıcı bilgilerini al (giriş yapıldıktan sonra tutulur)
    const storedUser = JSON.parse(localStorage.getItem("userData"));
    if (storedUser) {
      setUserData(storedUser);
    } /*else {
      // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
      navigate("/login");
    }*/
  }, [navigate]);

  const handleSurveyEdit = () => {
    if (userData.storeId) {
      // Kullanıcıya ait mağaza koduna göre düzenleme sayfasına yönlendir
      navigate(`/survey-edit/${userData.storeId}`);
    } else {
      alert("Store bilgisi bulunamadı!");
    }
  };

  const handleSurveyResults = () => {
    // Anket sonuçları sayfasına yönlendir
    navigate("/survey-results");
  };

  return (
    <div style={{ textAlign: "center", padding: '1%' }}>
      <h1 style={{ color: "#1c45b0", marginBottom: "10%", fontSize: 'clamp(1.5rem, 5vw, 3rem)' }}>
        - Admin Paneli -
      </h1>
      <div style={{ display: "flex", flexDirection: "column", gap: "20px" }}>
        <button
          onClick={handleSurveyResults}
          style={{
            width: "80%",
            margin: "0 auto",
            padding: "10px",
            fontSize: "18px",
            fontWeight: "bold",
            backgroundColor: "#000",
            color: "#fff",
            borderRadius: "5px",
            border: "2px solid #000",
            cursor: "pointer",
          }}
        >
          Anket Sonuçları
        </button>
        <button
          onClick={handleSurveyEdit}
          style={{
            width: "80%",
            margin: "0 auto",
            padding: "10px",
            fontSize: "18px",
            fontWeight: "bold",
            backgroundColor: "#000",
            color: "#fff",
            borderRadius: "5px",
            border: "2px solid #000",
            cursor: "pointer",
          }}
        >
          Anket Formu Düzenle
        </button>
      </div>
    </div>
  );
};

export default AdminPanel;
