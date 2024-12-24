import React, { useState, useEffect } from "react";
import axios from "axios";

const UserEdit = () => {
  const [users, setUsers] = useState([]);
  const [newUser, setNewUser] = useState({
    userMail: "",
    password: "",
    storeIds: [], // storeId yerine storeIds kullanımı
  });
  const [error, setError] = useState("");

  const baseURL = "http://localhost:5139/api";

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await axios.get(`${baseURL}/user/get-all-users`);
      setUsers(response.data);
    } catch (err) {
      console.error("Kullanıcılar yüklenemedi:", err);
    }
  };

  const handleAddUser = async () => {
    try {
      const userPayload = { ...newUser, storeIds: newUser.storeIds.map(Number) };
      await axios.post(`${baseURL}/user/add-user`, userPayload);
      fetchUsers();
      setNewUser({ userMail: "", password: "", storeIds: [] });
    } catch (err) {
      setError("Kullanıcı eklenirken bir hata oluştu.");
      console.error(err);
    }
  };

  const handleDeleteUser = async (id) => {
    try {
      await axios.delete(`${baseURL}/user/delete-user/${id}`);
      fetchUsers();
    } catch (err) {
      console.error("Kullanıcı silinirken bir hata oluştu:", err);
    }
  };

  const handleUpdateUser = async (id, user) => {
    try {
      const updatedMail = prompt("Yeni E-posta:", user.userMail);
      const updatedPassword = prompt("Yeni Şifre (boş bırakılırsa değişmez):", "");
      const updatedStoreIds = prompt("Yeni Store IDs (virgülle ayırın):",
        user.storeIds.join(",")
      );

      const updatedUser = {
        userMail: updatedMail || user.userMail, // Kullanıcı e-postası değişmemişse eski e-postayı kullan
        ...(updatedPassword && { password: updatedPassword }), // Şifre yalnızca bir değer girilmişse eklenir
        storeIds: updatedStoreIds
          ? updatedStoreIds.split(",").map(Number) // Virgülle ayrılmış stringi listeye dönüştür
          : user.storeIds, // Yeni bir StoreId girilmediyse eski listeyi kullan
      };      

      await axios.put(`${baseURL}/user/update-user/${id}`, updatedUser);
      fetchUsers();
    } catch (err) {
      console.error("Kullanıcı güncellenirken bir hata oluştu:", err);
      alert("Güncelleme sırasında bir hata oluştu.");
    }
  };

  return (
    <div style={{ textAlign: "center", padding: "20px" }}>
      <h1
        style={{
          color: "#1c45b0",
          marginBottom: "20px",
          fontSize: "clamp(1.5rem, 5vw, 3rem)",
        }}
      >
        Kullanıcı Yönetimi
      </h1>

      <div
        style={{
          border: "2px solid #000",
          borderRadius: "10px",
          padding: "20px",
          maxWidth: "100%",
          margin: "0 auto",
          backgroundColor: "#f7f7f7",
        }}
      >
        <h2 style={{ color: "#1c45b0" }}>Yeni Kullanıcı Ekle</h2>
        <input
          type="email"
          placeholder="E-posta"
          value={newUser.userMail}
          onChange={(e) =>
            setNewUser({ ...newUser, userMail: e.target.value })
          }
          style={{
            width: "90%",
            padding: "10px",
            marginBottom: "10px",
            borderRadius: "5px",
            border: "1px solid #ccc",
          }}
        />
        <input
          type="password"
          placeholder="Şifre"
          value={newUser.password}
          onChange={(e) =>
            setNewUser({ ...newUser, password: e.target.value })
          }
          style={{
            width: "90%",
            padding: "10px",
            marginBottom: "10px",
            borderRadius: "5px",
            border: "1px solid #ccc",
          }}
        />
        <input
          type="text"
          placeholder="Store IDs (virgülle ayırın)"
          value={newUser.storeIds.join(",")}
          onChange={(e) =>
            setNewUser({
              ...newUser,
              storeIds: e.target.value.split(",").map(Number),
            })
          }
          style={{
            width: "90%",
            padding: "10px",
            marginBottom: "10px",
            borderRadius: "5px",
            border: "1px solid #ccc",
          }}
        />
        <button
          onClick={handleAddUser}
          style={{
            backgroundColor: "#1c45b0",
            width: "30%",
            color: "#fff",
            padding: "10px 20px",
            border: "none",
            borderRadius: "5px",
            cursor: "pointer",
          }}
        >
          Ekle
        </button>
      </div>

      <h2 style={{ marginTop: "40px", color: "#1c45b0" }}>Mevcut Kullanıcılar</h2>
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          gap: "10px",
          maxWidth: "100%",
          margin: "20px auto",
        }}
      >
        {users.map((user) => (
          <div
            key={user.userId}
            style={{
              border: "2px solid #2196f3",
              borderRadius: "10px",
              padding: "15px",
              backgroundColor: "#ffffff",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
            }}
          >
            <div style={{ textAlign: "left", flex: 1 }}>
              <p style={{ margin: "0", fontWeight: "bold" }}>
                E-posta: {user.userMail}
              </p>
              <p style={{ margin: "5px 0 0" }}>
                Store IDs: {user.storeIds.join(", ")}
              </p>
            </div>
            <div style={{ display: "flex", gap: "10px" }}>
              <button
                onClick={() => handleDeleteUser(user.userId)}
                style={{
                  backgroundColor: "#f44336",
                  color: "#fff",
                  border: "none",
                  borderRadius: "5px",
                  padding: "5px 10px",
                  cursor: "pointer",
                }}
              >
                Sil
              </button>
              <button
                onClick={() => handleUpdateUser(user.userId, user)}
                style={{
                  backgroundColor: "#4caf50",
                  color: "#fff",
                  border: "none",
                  borderRadius: "5px",
                  padding: "5px 10px",
                  cursor: "pointer",
                }}
              >
                Güncelle
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default UserEdit;
