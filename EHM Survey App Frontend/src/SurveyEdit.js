import React, { useState, useEffect } from "react";
import axios from "axios";
import { useParams } from "react-router-dom";
import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";

const SurveyEdit = () => {
  const { StoreCode } = useParams();
  const [questions, setQuestions] = useState([]);
  const [storeId, setStoreId] = useState(null);
  const [newQuestion, setNewQuestion] = useState({
    question: "",
    questionType: "text",
    isActive: true,
    isRequired: true,
    questionOptions: "",
  });
  const [error, setError] = useState("");

  const baseURL = "http://localhost:5139/api";

  // Soruları ve mağaza bilgisini getir
  useEffect(() => {
    const fetchQuestions = async () => {
      try {
        const response = await axios.get(`${baseURL}/survey/get-survey/${StoreCode}`);
        if (response.status === 200) {
          setQuestions(response.data); 
          setStoreId(response.data[0]?.storeId || null); 
        } else {
          setError("Mağaza bulunamadı veya sorular yüklenemedi.");
        }
      } catch (err) {
        setError("Bir hata oluştu. Lütfen tekrar deneyiniz.");
        console.error(err);
      }
    };
  
    fetchQuestions();
  }, [StoreCode]);

  // Yeni soru ekle
  const handleAddQuestion = async () => {
    if (!newQuestion.question.trim()) {
      setError("Lütfen geçerli bir soru girin.");
      return;
    }

    try {
      const response = await axios.post(`${baseURL}/survey/add-survey`, {
        ...newQuestion,
        storeId,
        order: questions.length + 1,
      });

      if (response.status === 200) {
        setQuestions([...questions, { ...newQuestion, surveyId: response.data.surveyId }]);
        setNewQuestion({
          question: "",
          questionType: "text",
          isActive: true,
          isRequired: true,
          questionOptions: "",
        });
        setError("");
      } else {
        setError("Soru eklenirken bir hata oluştu.");
      }
    } catch (err) {
      setError("Bir hata oluştu. Lütfen tekrar deneyiniz.");
      console.error(err);
    }
  };

  // Soru sil
  const handleDeleteQuestion = async (questionId) => {
    try {
      const response = await axios.delete(`${baseURL}/survey/delete-survey/${questionId}`);
      if (response.status === 200) {
        setQuestions(questions.filter((q) => q.surveyId !== questionId));
      } else {
        setError("Soru silinirken bir hata oluştu.");
      }
    } catch (err) {
      setError("Bir hata oluştu. Lütfen tekrar deneyiniz.");
      console.error(err);
    }
  };

  // Soru güncelle
  const handleUpdateQuestion = async (questionId, updatedQuestion) => {
    try {
      const response = await axios.put(`${baseURL}/survey/update-survey/${questionId}`, updatedQuestion);
      if (response.status === 200) {
        setQuestions((prevQuestions) =>
          prevQuestions.map((q) =>
            q.surveyId === questionId ? { ...q, ...updatedQuestion } : q
          )
        );
      } else {
        setError("Soru güncellenirken bir hata oluştu.");
      }
    } catch (err) {
      setError("Bir hata oluştu. Lütfen tekrar deneyiniz.");
      console.error(err);
    }
  };

  // Soruların sırasını güncelle
  const handleDragEnd = async (result) => {
    if (!result.destination || questions.length === 0) return;
  
    const reorderedQuestions = Array.from(questions);
    const [movedItem] = reorderedQuestions.splice(result.source.index, 1);
    reorderedQuestions.splice(result.destination.index, 0, movedItem);
  
    setQuestions(reorderedQuestions);
  
    try {
      const reorderedData = reorderedQuestions.map((q, index) => ({
        surveyId: q.surveyId, // Backend'teki SurveyId
        order: index + 1,     // Yeni sıra numarası
      }));
  
      console.log("Gönderilen veri:", reorderedData); // Test için 
      await axios.patch(`${baseURL}/survey/update-survey-order`, reorderedData);
    } catch (err) {
      setError("Sıra güncellenirken bir hata oluştu.");
      console.error(err);
    }
  };
  
  return (
    <div style={{ padding: "20px" }}>
      <h1 style={{color: "#1c45b0"}}>Anket Soruları Düzenle - {StoreCode}</h1>
      {error && <p style={{ color: "red" }}>{error}</p>}

      <DragDropContext onDragEnd={handleDragEnd}>
  {questions.length > 0 ? (
    <Droppable droppableId="questions">
      {(provided) => (
        <div {...provided.droppableProps} ref={provided.innerRef}>
          {questions.map((question, index) => (
            <Draggable
              key={String(question.surveyId)}
              draggableId={String(question.surveyId)}
              index={index}
            >
              {(provided, snapshot) => (
                <div
                  ref={provided.innerRef}
                  {...provided.draggableProps}
                  {...provided.dragHandleProps}
                  style={{
                    ...provided.draggableProps.style,
                    border: "2px solid #2196f3",
                    borderRadius: "8px",
                    marginBottom: "15px",
                    padding: "15px",
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    backgroundColor: snapshot.isDragging
                      ? "#e3f2fd"
                      : "#ffffff",
                    boxShadow: snapshot.isDragging
                      ? "0 5px 10px rgba(0, 0, 0, 0.2)"
                      : "0 2px 4px rgba(0, 0, 0, 0.1)",
                    transition: "transform 0.2s ease-in-out",
                  }}
                >
                  {/* Sol tarafta soru detayları */}
                  <div 
                  className="question-details"
                  style={{ flex: 1 }}
                  >
                    <p style={{ margin: "0", fontWeight: "bold" }}>{question.question}</p>
                          <p style={{ margin: "5px 0 0" }}>Type: {question.questionType}</p>
                          <p style={{ margin: "5px 0 0" }}>
                            Aktif: {question.isActive ? "Evet" : "Hayır"}
                          </p>
                  </div>

                  {/* Sağ tarafta küçük butonlar */}
                  <div 
                  style={{ display: "flex", gap: "5px" }}
                  className="question-actions">
                    <button
                      onClick={() => handleDeleteQuestion(question.surveyId)}
                      style={{
                        backgroundColor: "#f44336",
                        color: "#fff",
                        border: "none",
                        borderRadius: "5px",
                        padding: "5px 10px",
                        cursor: "pointer",
                        fontSize: "0.8rem",
                      }}
                    >
                      Sil
                    </button>
                    <button
                      className="deactivate"
                      style={{
                        backgroundColor: question.isActive ? "#757575" : "#4caf50",
                        color: "#fff",
                        border: "none",
                        borderRadius: "5px",
                        padding: "5px 10px",
                        cursor: "pointer",
                        fontSize: "0.8rem",
                      }}
                      onClick={() =>
                        handleUpdateQuestion(question.surveyId, {
                          ...question,
                          isActive: !question.isActive,
                        })
                      }
                    >
                      {question.isActive ? "Deaktif Et" : "Aktif Et"}
                    </button>
                  </div>
                </div>
              )}
            </Draggable>
          ))}
          {provided.placeholder}
        </div>
      )}
    </Droppable>
  ) : (
    <p>Yüklenmiş soru bulunmamaktadır.</p>
  )}
</DragDropContext>

      {/* Yeni soru ekleme bölümü */}

      <div style={{ marginTop: "20px" }}>
        <h2>Yeni Soru Ekle</h2>
        <input
          type="text"
          placeholder="Soru"
          value={newQuestion.question}
          onChange={(e) =>
            setNewQuestion({ ...newQuestion, question: e.target.value })
          }
          style={{ width: "100%", padding: "10px", marginBottom: "10px" }}
        />
        <select
          value={newQuestion.questionType}
          onChange={(e) =>
            setNewQuestion({ ...newQuestion, questionType: e.target.value })
          }
          style={{ width: "100%", padding: "10px", marginBottom: "10px" }}
        >
          <option value="text">Text</option>
          <option value="radio">Radio</option>
          <option value="checkbox">Checkbox</option>
          <option value="rating">Rating</option>
        </select>
        <textarea
          placeholder="Seçenekler (virgülle ayrılmış)"
          value={newQuestion.questionOptions}
          onChange={(e) =>
            setNewQuestion({ ...newQuestion, questionOptions: e.target.value })
          }
          rows="5"
          cols="50"
          style={{ width: "100%", padding: "10px", marginBottom: "10px" }}
        ></textarea>
        <button
          onClick={handleAddQuestion}
          style={{
            backgroundColor: "blue",
            color: "white",
            padding: "10px 20px",
            border: "none",
            borderRadius: "5px",
            cursor: "pointer",
          }}
        >
          Soru Ekle
        </button>
      </div>
    </div>
  );
};

export default SurveyEdit;
