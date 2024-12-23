import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const AdminPanel = () => {
  const navigate = useNavigate();

  // Kullanıcı bilgilerini state'te tut
  const [userData, setUserData] = useState({
    userMail: "",
    storeId: null,
  });

  useEffect(() => {
    const storedUser = JSON.parse(localStorage.getItem("userData"));
    console.log("Stored User:", storedUser); // Debug için ekleyin
    if (storedUser) {
      setUserData(storedUser);
    } else {
      alert("LocalStorage'ta kullanıcı bilgisi bulunamadı.");
    }
  }, [navigate]);  

  const handleSurveyEdit = async () => {
    if (userData.storeId) {
      try {
        console.log("StoreId:", userData.storeId); // StoreId'yi kontrol et
        const response = await fetch(
          `http://localhost:5139/api/store/get-storecode/${userData.storeId}`
        );
  
        console.log("Response Status:", response.status); // HTTP durum kodu
        const data = await response.json();
        console.log("Backend Response:", data); // Gelen cevabı kontrol et
  
        if (response.ok) {
          navigate(`/survey-edit/${data.storeCode}`);
        } else {
          console.error("Hata:", data.message);
          alert("StoreCode bulunamadı!");
        }
      } catch (error) {
        console.error("Fetch Hatası:", error.message);
        alert("Bir hata oluştu. Lütfen tekrar deneyiniz.");
      }
    } else {
      console.error("userData.storeId bulunamadı.");
      alert("Store bilgisi bulunamadı!");
    }
  };
  

  const handleSurveyResults = () => {
    // Anket sonuçları sayfasına yönlendir
    navigate("/survey-results");
  };

  return (
    <div style={{ textAlign: "center", padding: '1%' }}>
      <h1 style={{
        color: "#1c45b0",
        marginTop: "0",
        marginBottom: "2%",
        fontSize: 'clamp(1.5rem, 5vw, 3rem)',
        textAlign: "center",
      }}>
        - Admin Paneli -
      </h1>
      <div style={{ display: "flex", flexDirection: "column", gap: "20px" }}>
        <button
          onClick={handleSurveyResults}
          style={{
            width: "80%",
            margin: "0 auto",
            padding: "4%",
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
            padding: "4%",
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
