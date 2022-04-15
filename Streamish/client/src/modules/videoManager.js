const baseUrl = '/api/video';

export const getAllVideos = () => {
  return fetch(`${baseUrl}/getwithcomments`)
    .then((res) => res.json())
};

export const addVideo = (video) => {
  return fetch(baseUrl, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(video),
  });
};

export const getVideo = (id) => {
  return fetch(`${baseUrl}/GetVideoByIdWithComments/${id}`).then((res) => res.json());
};