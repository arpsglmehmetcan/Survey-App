import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const AdminPanel = () => {
  const navigate = useNavigate();

  // Kullanıcı bilgilerini state'te tut
  const [userData, setUserData] = useState({
    userMail: "",
    storeId: null,
    storeIds: [], // Birden fazla StoreId destekleniyor
  });

  // Kullanıcı bilgilerini state'te set et
  useEffect(() => {
    const storedUser = JSON.parse(localStorage.getItem("userData"));
    console.log("Stored User:", storedUser); // Debug için ekleyin
    if (storedUser) {
      setUserData({
        userMail: storedUser.userMail,
        storeIds: storedUser.storeIds || [], // Varsayılan boş dizi
      });
    } else {
      alert("LocalStorage'ta kullanıcı bilgisi bulunamadı.");
    }
  }, [navigate]);

  const handleSurveyEdit = async (storeId) => {
    if (storeId) {
      try {
        console.log("StoreId:", storeId);
        const response = await fetch(
          `http://localhost:5139/api/store/get-storecode/${storeId}`
        );

        if (response.ok) {
          const data = await response.json();
          console.log("Backend Response:", data);
          navigate(`/survey-edit/${data.storeCode}`);
        } else {
          const errorData = await response.json();
          console.error("Hata:", errorData.message);
          alert("StoreCode bulunamadı!");
        }
      } catch (error) {
        console.error("Fetch Hatası:", error.message);
        alert("Bir hata oluştu. Lütfen tekrar deneyiniz.");
      }
    } else {
      console.error("StoreId bulunamadı.");
      alert("Store bilgisi bulunamadı!");
    }
  };

  const handleSurveyResults = () => {
    // Anket sonuçları sayfasına yönlendir
    navigate("/survey-results");
  };

  const handleUserEdit = () => {
    // Kullanıcı düzenleme sayfasına yönlendir
    navigate("/user-edit");
  };

  return (
    <div style={{ textAlign: "center", padding: "1%" }}>
      <h1
        style={{
          color: "#1c45b0",
          marginTop: "0",
          marginBottom: "2%",
          fontSize: "clamp(1.5rem, 5vw, 3rem)",
          textAlign: "center",
        }}
      >
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
        {userData.storeIds.length > 0 &&
          userData.storeIds.map((storeId) => (
            <button
              key={storeId}
              onClick={() => handleSurveyEdit(storeId)}
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
              Store {storeId} Anket Formu Düzenle
            </button>
          ))}
        {/* Sadece admin@gmail.com için Kullanıcı Düzenle butonu göster */}
        {userData.userMail === "admin@gmail.com" && (
          <button
            onClick={handleUserEdit}
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
            Kullanıcı Düzenle
          </button>
        )}
      </div>
    </div>
  );
};

export default AdminPanel;
